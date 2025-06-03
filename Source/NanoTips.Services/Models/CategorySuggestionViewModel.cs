using NanoTips.Data.Entities;

namespace NanoTips.Services.Models;

public record CategorySuggestionViewModel
{
    public required string CategoryId { get; init; }
    public required string CategorySlug { get; init; }
    public required double Confidence { get; init; }

    public string? Title { get; init; }
    public bool Exists => this.Title != null;

    public static CategorySuggestionViewModel? FromKnowledgeBaseArticle(KnowledgeBaseArticle? article, double confidence)
    {
        if(article == null || string.IsNullOrEmpty(article.Id.ToString()) || string.IsNullOrEmpty(article.Title))
            return null;
        
        return new CategorySuggestionViewModel
        {
            CategoryId = article.Id.ToString(),
            CategorySlug = article.Slug,
            Title = article.Title,
            Confidence = confidence,
        };
    }
}