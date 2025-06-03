using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NanoTips.Data.Entities;

public record KnowledgeBaseArticle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId Id { get; init; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId MailboxId { get; init; }
    
    public required string Slug { get; init; }
    
    public required string Title { get; init; }
    
    public required string Body { get; init; }
}