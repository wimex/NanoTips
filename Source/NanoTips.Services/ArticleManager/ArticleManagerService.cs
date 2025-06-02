using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public class ArticleManagerService(IMongoDatabase database) : IArticleManagerService
{
    public async Task<IList<ArticleListViewModel>> GetArticles()
    {
        IMongoCollection<KnowledgeBaseArticle> collection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        List<KnowledgeBaseArticle> articles = await collection.Find(FilterDefinition<KnowledgeBaseArticle>.Empty).ToListAsync();
        return articles.Select(x => new ArticleListViewModel
        {
            ArticleId = x.Id.ToString(),
            Slug = x.Slug,
            Title = x.Title,
        }).ToList();
    }
}