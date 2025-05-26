using MongoDB.Bson;

namespace NanoTips.Services.WebhookData;

public interface IWebhookDataService
{
    /// <summary>
    /// Saves the incoming RAW webhook data to the database without any processing.
    /// </summary>
    /// <param name="data"></param>
    Task SaveIncomingWebhookData(ObjectId id, string data);
}