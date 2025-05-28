using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Data.Enums;
using NanoTips.Services.Models;

namespace NanoTips.Services.ConversationManager;

public class ConversationManagerService(IMongoDatabase database) : IConversationManagerService
{
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