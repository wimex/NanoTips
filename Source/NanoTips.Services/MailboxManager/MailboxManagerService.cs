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
        if (string.IsNullOrWhiteSpace(model.PostmarkApiKey))
            throw new ArgumentException("Postmark API key cannot be null or empty.", nameof(model.PostmarkApiKey));
        if (!model.OpenAiApiKey.StartsWith("sk-") || model.OpenAiApiKey.Length < 160)
            throw new ArgumentException("OpenAI API key must start with 'sk-' and be at least 160 characters long.", nameof(model.OpenAiApiKey));
        if(!Guid.TryParse(model.PostmarkApiKey, out _))
            throw new ArgumentException("Postmark API key must be a valid GUID.", nameof(model.PostmarkApiKey));
        
        IMongoCollection<SystemMailbox> collection = database.GetCollection<SystemMailbox>(NanoTipsCollections.SystemMailboxes);
        SystemMailbox mailbox = new()
        {
            Id = ObjectId.GenerateNewId(),
            OpenAiApiKey = model.OpenAiApiKey,
            PostmarkApiKey = model.PostmarkApiKey,
        };
        
        await collection.InsertOneAsync(mailbox);
        
        return new MailboxViewModel
        {
            MailboxId = mailbox.Id.ToString()
        };
    }
}