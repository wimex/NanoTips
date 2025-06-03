using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Data.Enums;
using NanoTips.Services.EmailResponder;
using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public class ConversationManagerService(IMongoDatabase database) : IConversationManagerService
{
    public async Task<ConversationViewModel> ReplyToConversation(string mailboxId, ConversationEditorModel model)
    {
        ObjectId conversationId = ObjectId.Parse(model.ConversationId);
        
        IMongoCollection<ConversationMessage> messages = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        IMongoCollection<KnowledgeBaseArticle> articles = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        
        KnowledgeBaseArticle? article = await articles.Find(a => a.MailboxId == ObjectId.Parse(mailboxId) && a.Slug == model.ArticleSlug).FirstOrDefaultAsync();
        if (article == null)
            throw new KeyNotFoundException($"Knowledge base article with slug '{model.ArticleSlug}' not found.");

        ConversationMessage last = await messages
            .Find(c => c.ConversationId == conversationId && c.Direction == MessageDirection.Incoming)
            .SortByDescending(c => c.Created)
            .FirstOrDefaultAsync();
        
        ConversationMessage message = new()
        {
            Id = ObjectId.GenerateNewId(),
            MailboxId = last.MailboxId,
            ConversationId = conversationId,
            Created = DateTime.UtcNow,
            Processed = null,
            Direction = MessageDirection.Outgoing,
            Sender = "NanoTips",
            Recipient = last.Sender,
            Subject = last.Subject,
            Body = article.Body,
            CategorySuggestions = new(),
            CategoryId = article.Slug,
        };
        
        await messages.InsertOneAsync(message);
        EmailResponderService.MailSenderCallback.Invoke(message.Id.ToString());
        
        return await this.GetConversation(model.ConversationId);
    }
    
    public async Task<ConversationViewModel> GetConversation(string conversationId)
    {
        if (!ObjectId.TryParse(conversationId, out ObjectId parsedConversationId)) throw new ArgumentException("Invalid conversation ID format.", nameof(conversationId));

        IMongoCollection<ConversationMessage> cmCollection = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        List<ConversationMessage>? messages = await cmCollection
            .Find(c => c.ConversationId == parsedConversationId)
            .SortByDescending(c => c.Created)
            .ToListAsync();

        if (messages.Count == 0)
            throw new KeyNotFoundException($"No messages found for conversation ID: {conversationId}");

        List<string> suggestionIds = messages
            .SelectMany(m => m.CategorySuggestions.Keys)
            .Distinct()
            .ToList();
        
        List<ObjectId> mailboxIds = messages
            .Select(m => m.MailboxId)
            .Distinct()
            .ToList();
        
        IMongoCollection<KnowledgeBaseArticle> kbCollection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        List<KnowledgeBaseArticle>? suggestions = await kbCollection
            .Find(k => mailboxIds.Contains(k.MailboxId) && suggestionIds.Contains(k.Slug.ToString()))
            .ToListAsync();
        
        ConversationViewModel conversation = new()
        {
            ConversationId = conversationId,
            Subject = messages.FirstOrDefault()?.Subject ?? "No Subject",
            Messages = messages.Select(m => new ConversationMessageViewModel
            {
                MessageId = m.Id.ToString(),
                Created = m.Created,
                Processed = m.Processed,
                Direction = m.Direction,
                Sender = m.Sender,
                Recipient = m.Recipient,
                Subject = m.Subject,
                Body = m.Body,
                CategorySuggestions = m.CategorySuggestions.ToDictionary(
                    kvp => kvp.Key,
                    kvp => CategorySuggestionViewModel.FromKnowledgeBaseArticle( 
                        suggestions.FirstOrDefault(s => s.Slug.ToString() == kvp.Key), kvp.Value)
                        ?? new CategorySuggestionViewModel{ CategoryId = kvp.Key, CategorySlug = kvp.Key, Title = null, Confidence = kvp.Value }
                ),
                CategoryId = m.CategoryId,
            }).ToList()
        };

        return conversation;
    }
    
    public async Task<IList<ConversationListModel>> GetConversations(string mailboxId)
    {
        IMongoCollection<ConversationMessage> collection = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        var conversations = await collection
            .Aggregate()
            .Match(c => c.MailboxId == ObjectId.Parse(mailboxId))
            .Group(c => c.ConversationId, g => new ConversationListModel
            {
                ConversationId = g.Key.ToString(),
                Subject = g.FirstOrDefault().Subject ?? "No Subject",
                MessageCount = g.Count(),
                Answered = !g.Any(c => c.Direction == MessageDirection.Incoming && c.Processed == null),
                LastMessageDate = g.Max(c => c.Created),
            })
            .ToListAsync();

        return conversations.OrderByDescending(c => c.LastMessageDate).ToList();
    }
}