using MongoDB.Bson;

namespace NanoTips.Data.Entities;

public class SystemMailbox
{
    public required ObjectId Id { get; set; }
    
    public required string OpenAiApiKey { get; set; }
}