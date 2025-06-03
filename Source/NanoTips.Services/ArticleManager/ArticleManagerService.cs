using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public class ArticleManagerService(IMongoDatabase database) : IArticleManagerService
{
    public async Task<ArticleViewModel> CreateOrEditArticle(string mailboxId, ArticleEditorModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        IMongoCollection<KnowledgeBaseArticle> collection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        if (model.ArticleId == null)
        {
            ObjectId articleId = ObjectId.GenerateNewId();
            await collection.InsertOneAsync(new KnowledgeBaseArticle
            {
                Id = articleId,
                MailboxId = ObjectId.Parse(mailboxId),
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
                    MailboxId = ObjectId.Parse(mailboxId),
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

    public async Task<ArticleViewModel> GetArticle(string articleId)
    {
        IMongoCollection<KnowledgeBaseArticle> collection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        KnowledgeBaseArticle? article = await collection.Find(x => x.Id == ObjectId.Parse(articleId)).FirstOrDefaultAsync();
        if (article == null)
        {
            throw new KeyNotFoundException($"Article with ID {articleId} not found.");
        }

        return new ArticleViewModel
        {
            ArticleId = article.Id.ToString(),
            Slug = article.Slug,
            Title = article.Title,
            Content = article.Body,
        };
    }
    
    public async Task<IList<ArticleListViewModel>> GetArticles(string mailboxId)
    {
        IMongoCollection<KnowledgeBaseArticle> collection = database.GetCollection<KnowledgeBaseArticle>(NanoTipsCollections.KnowledgeBaseArticles);
        List<KnowledgeBaseArticle> articles = await collection.Find(x => x.MailboxId == ObjectId.Parse(mailboxId))
            .SortBy(x => x.Title)
            .ToListAsync();
        
        return articles.Select(x => new ArticleListViewModel
        {
            ArticleId = x.Id.ToString(),
            Slug = x.Slug,
            Title = x.Title,
        }).ToList();
    }
}