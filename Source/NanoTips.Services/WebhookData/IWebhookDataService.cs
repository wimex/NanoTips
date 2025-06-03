using MongoDB.Bson;

namespace NanoTips.Services.WebhookData;

public interface IWebhookDataService
{
    /// <summary>
    /// Saves the incoming RAW webhook data to the database without any processing.
    /// </summary>
    /// <param name="data"></param>
    Task SaveIncomingWebhookData(ObjectId mailboxId, ObjectId id, string data);

    /// <summary>
    /// Creates a conversation message from an incoming webhook message.
    /// </summary>
    /// <param name="webhookMessageId"></param>
    /// <param name="conversationId"></param>
    /// <param name="conversationMessageId"></param>
    /// <returns></returns>
    Task CreateConversationFromMessage(ObjectId mailboxId, ObjectId webhookMessageId, ObjectId conversationId, ObjectId conversationMessageId);
}