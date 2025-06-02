using MongoDB.Bson;
using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public interface IArticleManagerService
{
    Task<ArticleViewModel> CreateOrEditArticle(ArticleEditorModel model);
    Task<ArticleViewModel> GetArticle(string articleId);
    Task<IList<ArticleListViewModel>> GetArticles();
}