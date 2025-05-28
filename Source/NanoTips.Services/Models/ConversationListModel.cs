using MongoDB.Bson;

namespace NanoTips.Services.Models;

public record ConversationListModel
{
    public required string ConversationId { get; init; }
    public required string Subject { get; init; }
    public required DateTime LastMessageDate { get; init; }
    public required int MessageCount { get; init; }
    public required bool Answered { get; init; }
}