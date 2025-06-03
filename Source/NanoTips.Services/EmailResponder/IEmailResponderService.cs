using MongoDB.Bson;

namespace NanoTips.Services.EmailResponder;

public interface IEmailResponderService
{
    Task TryRespondingToMail(ObjectId mailboxId, ObjectId messageId);
}