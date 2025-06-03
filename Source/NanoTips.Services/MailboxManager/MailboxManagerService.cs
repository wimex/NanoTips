using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.MailboxManager;

public class MailboxManagerService(IMongoDatabase database) : IMailboxManagerService
{
    public async Task<MailboxViewModel> RegisterMailbox(MailboxEditorModel model)
    {
        if (string.IsNullOrWhiteSpace(model.OpenAiApiKey))
            throw new ArgumentException("OpenAI API key cannot be null or empty.", nameof(model.OpenAiApiKey));
        
        IMongoCollection<SystemMailbox> collection = database.GetCollection<SystemMailbox>(NanoTipsCollections.SystemMailboxes);
        SystemMailbox mailbox = new()
        {
            Id = ObjectId.GenerateNewId(),
            OpenAiApiKey = model.OpenAiApiKey
        };
        
        await collection.InsertOneAsync(mailbox);
        
        return new MailboxViewModel
        {
            MailboxId = mailbox.Id.ToString()
        };
    }
}