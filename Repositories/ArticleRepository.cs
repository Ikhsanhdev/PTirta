using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IArticleRepository
{

    Task<User?> GetByIdAsync(Guid id);
    Task<List<Article>> GetListArticleAsync();
    Task<Article?> GetArticleBySlugAsync(string slug);
    Task<List<Article>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataArticle(JqueryDataTableRequest request);
    Task<User?> GetByTitleAsync(string username);
    Task<ArticleVM> GetArticleByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(ArticleVM article);
    Task<Article?> UpdateArticleAsync(Article article);
    Task<bool> DeleteArticleAsync(Guid id);
}

public class ArticleRepository : IArticleRepository
{
    private readonly string _connectionString;
    public ArticleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            string query = @"SELECT
                    id AS ""Id"",
                    username AS ""Username"",
                    name AS ""Name"",
                    email AS ""Email"",
                    phone AS ""Phone"",
                    password AS ""Password"",
                    last_login AS ""LastLogin""
                FROM users WHERE id = @id";

            using var connection = new NpgsqlConnection(_connectionString);
            User? user = await connection.QueryFirstOrDefaultAsync<User>(query, new { id });
            return user;
        }
        catch (NpgsqlException)
        {
            throw;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<User?> GetByTitleAsync(string username)
    {
        try
        {
            string query = @"SELECT
                    id AS ""Id"",
                    username AS ""Username"",
                    name AS ""Name"",
                    email AS ""Email"",
                    phone AS ""Phone"",
                    password AS ""Password"",
                    last_login AS ""LastLogin""
                FROM users WHERE username = @username";

            using var connection = new NpgsqlConnection(_connectionString);
            User? user = await connection.QueryFirstOrDefaultAsync<User>(query, new { username });
            return user;
        }
        catch (NpgsqlException)
        {
            throw;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<AjaxResponse> SaveAsync(ArticleVM article)
    {
        AjaxResponse result = new();
        try
        {
            string query = "";
            string status = "Tambah";

            if (article.id == Guid.Empty)
            {
                query = @"
                    INSERT INTO 
                    articles ( title, description, author, img_url, category, slug)
                    VALUES ( @title, @description, @author, @img_url, @category , LOWER(REPLACE(@title, ' ', '-')))
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                article.updated_at = DateTime.Now;
                query = @"
                        UPDATE articles
                        SET title = @title, description = @description, author = @author, img_url = @img_url, updated_at = @updated_at, category = @category , slug = LOWER(REPLACE(@title, ' ', '-'))
                        WHERE id = @Id
                        RETURNING *;";

            }


            using (var connection = new NpgsqlConnection(_connectionString))
            {

                var data = await connection.ExecuteAsync(query, article);

                result.Code = 200;
                result.Message = $"{status} data berhasil";
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving article: {ex.Message}");
            result.Code = 500;
            result.Message = "Terjadi Kesalahan saat menyimpan data";
        }
        return result;
    }

    public async Task<Article?> UpdateArticleAsync(Article article)
    {
        const string query = @"
            UPDATE articles
            SET title = @Title, deskripsi = @Deskripsi, penulis = @Penulis, gambar_url = @GambarUrl, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Article>(query, article);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating article: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteArticleAsync(Guid id)
    {
        const string query = @"
            UPDATE articles
            SET deleted_at = @Date_now
            WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int affectedRows = await connection.ExecuteAsync(query, new { Id = id, Date_now = DateTime.Now });
                return affectedRows > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting article: {ex.Message}");
            return false;
        }
    }


    public async Task<ArticleVM> GetArticleByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM articles WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var article = await connection.QuerySingleOrDefaultAsync<ArticleVM>(query, new { Id = id });
                return article;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Article>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @$"
                        SELECT
                            id AS ""Id"",
                            title AS ""Title"",
                            description AS ""Description"",
                            author AS ""Author"",
                            img_url AS ""ImgUrl"",
                            created_at AS ""CreatedAt"",
                            updated_at AS ""UpdatedAt"";
                        FROM articles
                        LIMIT 10
                        ORDER BY created_at DESC 
                        ;";

            var result = await connection.QueryAsync<Article>(query);
            return result.ToList();
        }
        catch (NpgsqlException ex)
        {
            Log.Error(ex, "PostgreSQL Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace });
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace });
            throw;
        }
    }

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataArticle(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    articles 
                    ";

            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                if (request.SearchValue.Contains('\''))
                {
                    request.SearchValue = request.SearchValue.Replace("'", "''");
                }


                whereConditions.Add(@"
                    (LOWER(title) LIKE @SearchValue OR
                    LOWER(author) LIKE @SearchValue OR
                    LOWER(description) LIKE @SearchValue)");
                parameters.Add("@SearchValue", "%" + request.SearchValue.ToLower() + "%");
            }

            // if (!string.IsNullOrEmpty(request.Status))
            // {
            //     whereConditions.Add(@"
            //     status = @Status");
            //     parameters.Add("@Status", request.Status);
            // }

            whereConditions.Add(@" deleted_at IS NULL");

            var whereClause = whereConditions.Count > 0 ? "WHERE" + string.Join(" AND ", whereConditions) : "";

            query += whereClause;

            query += @" ORDER BY created_at DESC";
            
            int total = 0;
            var sql_count = $"SELECT COUNT(*) FROM ({query}) as total";
            total = connection.ExecuteScalar<int>(sql_count, parameters);

            query += @" 
                OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;";
            parameters.Add("@Skip", request.Skip);
            parameters.Add("@PageSize", request.PageSize);

            result = (await connection.QueryAsync<dynamic>(query, parameters)).ToList();

            return (result, total);
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Log.Error(ex, "Sql Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, Desc = "Error while get data to table petak" });
            throw;
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace, Desc = "Error while get data to table petak" });
            throw;
        }
    }

    public async Task<List<Article>> GetListArticleAsync()
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                title AS ""Title"",
                description AS ""Description"",
                author AS ""Author"",
                category AS ""Category"",
                img_url AS ""Image"",
                created_at AS ""CreatedAt"",
                slug AS ""Slug""
            FROM articles
            WHERE deleted_at IS NULL 
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var article = await connection.QueryAsync<Article>(query);
                return article.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article: {ex.Message}");
            return null;
        }
    }

    public async Task<Article?> GetArticleBySlugAsync(string slug)
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                title AS ""Title"",
                description AS ""Description"",
                author AS ""Author"",
                category AS ""Category"",
                img_url AS ""Image"",
                created_at AS ""CreatedAt""
            FROM articles
            WHERE slug = @Slug
            AND deleted_at IS NULL;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Article? model = await connection.QuerySingleOrDefaultAsync<Article>(query, new { Slug = slug });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article: {ex.Message}");
            return null;
        }
    }
}