using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Data.Enums;

namespace NanoTips.Services.WebhookData;

public class WebhookDataService(IMongoDatabase database) : IWebhookDataService
{
    /// <summary>
    /// Saves the incoming RAW webhook data to the database without any processing.
    /// </summary>
    /// <param name="data"></param>
    public async Task SaveIncomingWebhookData(ObjectId mailboxId, ObjectId id, string data)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrEmpty(data);

        BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(data);
        if (document == null)
            throw new InvalidOperationException("Deserialized document cannot be null.");

        IMongoCollection<BsonDocument> messages = database.GetCollection<BsonDocument>(NanoTipsCollections.WebhookMessages);
        document["_id"] = id;
        document["MailboxId"] = mailboxId;
        
        await messages.InsertOneAsync(document);
    }
    
    public async Task CreateConversationFromMessage(ObjectId mailboxId, ObjectId webhookMessageId, ObjectId conversationId, ObjectId conversationMessageId)
    {
        IMongoCollection<BsonDocument> messages = database.GetCollection<BsonDocument>(NanoTipsCollections.WebhookMessages);
        IMongoCollection<ConversationMessage> conversations = database.GetCollection<ConversationMessage>(NanoTipsCollections.ConversationMessages);
        
        BsonDocument message = await (await messages.FindAsync(m => m["_id"] == webhookMessageId)).FirstOrDefaultAsync();
        if (message == null)
            throw new InvalidOperationException($"Message with ID {webhookMessageId} was not found.");

        string sender = message["From"]?.AsString;
        string recipient = message["To"]?.AsString;
        if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(recipient))
            throw new InvalidOperationException("Message does not contain valid sender or recipient information.");

        string subject = message["Subject"]?.AsString;
        string body = message["HtmlBody"]?.AsString ?? message["TextBody"]?.AsString;
        if (string.IsNullOrEmpty(body))
            throw new InvalidOperationException("Message does not contain a body.");
        
        ConversationMessage conversation = new ConversationMessage
        {
            Id = conversationMessageId,
            MailboxId = mailboxId,
            ConversationId = conversationId,
            Created = DateTime.UtcNow,
            Direction = MessageDirection.Incoming,
            Sender = sender,
            Recipient = recipient,
            Subject = !string.IsNullOrEmpty(subject) ? subject : "No Subject",
            Body = body,
        };
        
        await conversations.InsertOneAsync(conversation);
    }
}