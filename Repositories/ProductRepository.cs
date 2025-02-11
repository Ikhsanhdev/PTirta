using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IProductRepository
{
    // Task<User?> GetByIdAsync(Guid id);
    // Task<List<Article>> GetAllAsync();
    // Task<(IReadOnlyList<dynamic>, int)> GetDataArticle(JqueryDataTableRequest request);
    // Task<User?> GetByTitleAsync(string username);
    // Task<ArticleVM?> GetArticleByIdAsync(Guid id);
    // Task<AjaxResponse> SaveAsync(ArticleVM article);
    // Task<Article?> UpdateArticleAsync(Article article);
    // Task<bool> DeleteArticleAsync(Guid id);
}

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;
    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
}