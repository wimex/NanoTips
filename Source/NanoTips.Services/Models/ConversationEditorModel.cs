namespace NanoTips.Services.Models;

public record ConversationEditorModel
{
    public required string ConversationId { get; init; }
    public required string ArticleSlug { get; init; }
}