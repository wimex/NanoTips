namespace NanoTips.Services.Models;

public record ArticleViewModel
{
    public required string ArticleId { get; init; }
    public required string Slug { get; init; }
    
    public required string Title { get; init; }
    public required string Content { get; init; }
}