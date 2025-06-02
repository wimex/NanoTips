namespace NanoTips.Services.Models;

public record ArticleEditorModel
{
    public string? ArticleId { get; init; }
    
    public required string Slug { get; init; }
    
    public required string Title { get; init; }
    
    public required string Content { get; init; }
}