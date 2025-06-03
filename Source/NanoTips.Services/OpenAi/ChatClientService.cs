using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using OpenAI.Chat;

namespace NanoTips.Services.OpenAi;

//TODO: add a message length limit
public class ChatClientService(ILogger<ChatClientService> logger, IMongoDatabase database) : IChatClientService
{
    public async Task<Dictionary<string, double>> GetEmailCategory(string mailboxId, string content)
    {
        Dictionary<string, double> empty = new();
        
        IMongoCollection<SystemMailbox> mailboxes = database.GetCollection<SystemMailbox>(NanoTipsCollections.SystemMailboxes);
        SystemMailbox? mailbox = await mailboxes.Find(m => m.Id == ObjectId.Parse(mailboxId)).FirstOrDefaultAsync();
        if (mailbox is null || string.IsNullOrEmpty(mailbox.OpenAiApiKey))
            throw new InvalidOperationException($"Mailbox with ID {mailboxId} was not found.");

        logger.LogInformation("Processing message for categorization ({length} characters)", content.Length);
        List<string> categories = await database
            .GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles)
            .Find(article => article.MailboxId == ObjectId.Parse(mailboxId) && !string.IsNullOrEmpty(article.Slug))
            .Project(article => article.Slug)
            .ToListAsync();

        logger.LogInformation("Loaded {Count} categories in the knowledge base.", categories.Count);
        string cats = string.Join(",", categories); //Meow
        string prompt = Prompt.Replace("{ids}", cats).Replace("{email}", content);
        
        logger.LogInformation("Sending prompt to OpenAI (using mailbox {MailboxId})", mailboxId);
        ChatClient client = new ChatClient("gpt-4o", mailbox.OpenAiApiKey);
        ChatMessage message = ChatMessage.CreateUserMessage(prompt);
        ClientResult<ChatCompletion> result = await client.CompleteChatAsync([message], new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        });
        
        logger.LogInformation("Received response from OpenAI");
        if (!string.IsNullOrEmpty(result.Value.Refusal) || result.Value.Content.Count <= 0)
            throw new InvalidOperationException($"OpenAI refused to process the request or returned no content: {result.Value.Refusal}");

        logger.LogInformation("Parsing response from OpenAI");
        string response = result.Value.Content[0].Text;
        if (string.IsNullOrEmpty(response))
            return empty;

        try
        {
            Dictionary<string, double> candidates = JsonSerializer.Deserialize<Dictionary<string, double>>(response);
            return candidates ?? empty;
        }
        catch (Exception ex)
        {
            logger.LogWarning("JSON parsing failed: {Message}", ex.Message);
            return empty;
        }
    }
    
    private const string Prompt = """
        You are an email classification bot. You have to process the following email and shorten it down to an ID.
        This ID is used as a category to figure out if multiple messages have the same contents.
        Try to reuse the IDs as much as possible, but create a new one when it's necessary.
        Between the square brackets, you'll find the existing IDs separated by commas: [{ids}]
        Whats the top 3 ID candidates for this email? Reply with ONLY a parsable JSON dictionary of smallercase IDs and probabilities.
        
        {email}
        """;
}