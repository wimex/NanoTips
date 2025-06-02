using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public class ArticleManagerService(IMongoDatabase database) : IArticleManagerService
{
    public async Task<ArticleViewModel> CreateOrEditArticle(ArticleEditorModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        IMongoCollection<KnowledgeBaseArticle> collection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        if (model.ArticleId == null)
        {
            ObjectId articleId = ObjectId.GenerateNewId();
            await collection.InsertOneAsync(new KnowledgeBaseArticle
            {
                Id = articleId,
                Slug = model.Slug,
                Title = model.Title,
                Body = model.Content,
            });
            
            return new ArticleViewModel
            {
                ArticleId = articleId.ToString(),
                Slug = model.Slug,
                Title = model.Title,
                Content = model.Content,
            };
        }
        else
        {
            ObjectId articleId = ObjectId.Parse(model.ArticleId);
            await collection.ReplaceOneAsync(
                x => x.Id == articleId,
                new KnowledgeBaseArticle
                {
                    Id = articleId,
                    Slug = model.Slug,
                    Title = model.Title,
                    Body = model.Content,
                });
            
            return new ArticleViewModel
            {
                ArticleId = articleId.ToString(),
                Slug = model.Slug,
                Title = model.Title,
                Content = model.Content,
            };
        }
    }
    
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