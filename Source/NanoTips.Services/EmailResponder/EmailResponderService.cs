using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Data.Enums;
using NanoTips.Services.OpenAi;

namespace NanoTips.Services.EmailResponder;

public class EmailResponderService(ILogger<EmailResponderService> logger, IMongoDatabase database, IChatClientService chatClientService) : IEmailResponderService
{
    public static Action<string> MailSenderCallback { get; set; } = _ => { };
    
    private const double DefaultProbabilityThreshold = 0.7;
    
    public async Task TryRespondingToMail(ObjectId mailboxId, ObjectId messageId)
    {
        ConversationMessage? message = await database
            .GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages)
            .Find(message => message.Id == messageId)
            .FirstOrDefaultAsync();

        if (message is null || string.IsNullOrEmpty(message.Body))
            throw new InvalidOperationException($"Message with ID {messageId} was not found or has no content.");
        
        Dictionary<string, double> categories = await chatClientService.GetEmailCategory(mailboxId.ToString(), message.Body);
        string? category = categories
            .Where(c => c.Value >= DefaultProbabilityThreshold)
            .OrderByDescending(c => c.Value)
            .Select(c => c.Key)
            .FirstOrDefault();
        
        logger.LogInformation("Suggested categories for message {MessageId}: {Categories}", messageId, string.Join(", ", categories.Select(c => $"{c.Key} ({c.Value:P2})")));
        await database
            .GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages)
            .UpdateOneAsync(m => m.Id == messageId, Builders<ConversationMessage>.Update.Set(m => m.CategorySuggestions, categories));

        if (string.IsNullOrEmpty(category))
        {
            logger.LogInformation("No suitable category found for message {MessageId}. No response will be sent.", messageId);
            return;
        }
        
        logger.LogInformation("Category '{Category}' selected for message {MessageId} with probability {Probability}.", category, messageId, categories[category]);
        KnowledgeBaseArticle? article = await database
            .GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles)
            .Find(article => article.MailboxId == mailboxId && article.Slug == category)
            .FirstOrDefaultAsync();

        if (article is null || string.IsNullOrEmpty(article.Body))
        {
            logger.LogWarning("No knowledge base article found for category '{Category}' in message {MessageId}.", category, messageId);
            return;
        }
        
        ConversationMessage reply = new()
        {
            Id = ObjectId.GenerateNewId(),
            MailboxId = message.MailboxId,
            ConversationId = message.ConversationId,
            Created = DateTime.UtcNow,
            Direction = MessageDirection.Outgoing,
            CategoryId = category,
            Sender = "NanoTips Bot",
            Recipient = message.Sender,
            Subject = message.Subject,
            Body = article.Body,
        };
        
        logger.LogInformation("Saving reply to message {MessageId}.", messageId);
        await database
            .GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages)
            .InsertOneAsync(reply);
        
        MailSenderCallback.Invoke(reply.Id.ToString());
    }
}