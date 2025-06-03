using MongoDB.Bson;

namespace NanoTips.Services.OpenAi;

public interface IChatClientService
{
    Task<Dictionary<string, double>> GetEmailCategory(string mailboxId, string content);
}