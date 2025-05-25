using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NanoTips.Data.Entities;

public record NtThread
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; init; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid ThreadId { get; init; }
}