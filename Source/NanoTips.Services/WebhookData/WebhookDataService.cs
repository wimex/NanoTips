using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NanoTips.Data;

namespace NanoTips.Services.WebhookData;

public class WebhookDataService(IMongoDatabase database) : IWebhookDataService
{
    /// <summary>
    /// Saves the incoming RAW webhook data to the database without any processing.
    /// </summary>
    /// <param name="data"></param>
    public async Task SaveIncomingWebhookData(ObjectId id, string data)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrEmpty(data);

        BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(data);
        if (document == null)
            throw new InvalidOperationException("Deserialized document cannot be null.");

        IMongoCollection<BsonDocument> messages = database.GetCollection<BsonDocument>(NanoTipsCollections.Messages);
        document["_id"] = id;
        
        await messages.InsertOneAsync(document);
    }
}