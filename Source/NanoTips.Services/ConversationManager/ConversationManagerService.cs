using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Data.Enums;
using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public class ConversationManagerService(IMongoDatabase database) : IConversationManagerService
{
    public async Task<ConversationViewModel> GetConversation(string conversationId)
    {
        if (!ObjectId.TryParse(conversationId, out ObjectId parsedConversationId)) throw new ArgumentException("Invalid conversation ID format.", nameof(conversationId));

        IMongoCollection<ConversationMessage> collection = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        List<ConversationMessage>? messages = await collection
            .Find(c => c.ConversationId == parsedConversationId)
            .SortByDescending(c => c.Created)
            .ToListAsync();

        if (messages.Count == 0)
            throw new KeyNotFoundException($"No messages found for conversation ID: {conversationId}");

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
                CategorySuggestions = m.CategorySuggestions,
                CategoryId = m.CategoryId,
            }).ToList()
        };

        return conversation;
    }
    
    public async Task<IList<ConversationListModel>> GetConversations()
    {
        IMongoCollection<ConversationMessage> collection = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        var conversations = await collection
            .Aggregate()
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