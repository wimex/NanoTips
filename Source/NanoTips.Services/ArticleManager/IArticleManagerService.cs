using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public interface IArticleManagerService
{
    Task<ArticleViewModel> CreateOrEditArticle(string mailboxId, ArticleEditorModel model);
    Task<ArticleViewModel> GetArticle(string articleId);
    Task<IList<ArticleListViewModel>> GetArticles(string mailboxId);
}