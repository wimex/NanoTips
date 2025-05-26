using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NanoTips.Data.Enums;

namespace NanoTips.Data.Entities;

public record ConversationMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId Id { get; init; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId ConversationId { get; init; }
    
    /// <summary>
    /// When the message was created or received through a webhook.
    /// </summary>
    public required DateTime Created { get; init; }
    
    /// <summary>
    /// When the message was sent or received.
    /// </summary>
    public DateTime? Processed { get; init; }
    
    /// <summary>
    /// Wether it's an outgoing or incoming message.
    /// </summary>
    public required MessageDirection Direction { get; init; }
    
    /// <summary>
    /// A category ID from the categories collection. Created by the AI to classify messages.
    /// </summary>
    public string? CategoryId { get; init; }
    
    /// <summary>
    /// The sender of the message (e-mail address)
    /// </summary>
    public required string Sender { get; init; }
    
    /// <summary>
    /// The recipient of the message (e-mail address)
    /// </summary>
    public required string Recipient { get; init; }
    
    public required string Subject { get; init; }
    public required string Body { get; init; }
}