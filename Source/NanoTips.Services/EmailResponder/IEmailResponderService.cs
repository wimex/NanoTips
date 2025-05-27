using MongoDB.Bson;

namespace NanoTips.Services.EmailResponder;

public interface IEmailResponderService
{
    Task TryRespondingToMail(ObjectId messageId);
}