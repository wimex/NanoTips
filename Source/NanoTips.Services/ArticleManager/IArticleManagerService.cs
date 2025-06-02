using MongoDB.Driver;
using NanoTips.Data.Entities;
using NanoTips.Services.Models;

namespace NanoTips.Services.ArticleManager;

public interface IArticleManagerService
{
    Task<IList<ArticleListViewModel>> GetArticles();
}