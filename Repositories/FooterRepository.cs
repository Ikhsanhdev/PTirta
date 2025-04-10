using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using TirtaRK.Models.Datatables;
using TirtaRK.ViewModels;

namespace TirtaRK.Repositories;

public interface IFooterRepository
{
    Task<List<Footer>> GetListFooterAsync();
    Task<Footer?> GetFooterBySlugAsync(string slug);
    Task<List<Footer>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataFooter(JqueryDataTableRequest request);
    Task<FooterVM?> GetFooterByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(FooterVM model);
    Task<Footer?> UpdateArticleAsync(Footer model);
    Task<bool> DeleteAsync(Guid id);
}

public class FooterRepository : IFooterRepository
{
    private readonly string _connectionString;
    public FooterRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<AjaxResponse> SaveAsync(FooterVM model)
    {
        AjaxResponse result = new();
        try
        {
            string query = "";
            string status = "Tambah";

            if (model.id == Guid.Empty)
            {
                query = @"
                    INSERT INTO 
                    footer_contact ( name, value)
                    VALUES ( @name, @value)
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                model.updated_at = DateTime.Now;
                query = @"
                        UPDATE footer_contact
                        SET name = @name, value = @value, updated_at = @updated_at
                        WHERE id = @Id
                        RETURNING *;";

            }


            using (var connection = new NpgsqlConnection(_connectionString))
            {

                var data = await connection.ExecuteAsync(query, model);

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

    public async Task<Footer?> UpdateArticleAsync(Footer model)
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
                return await connection.QueryFirstOrDefaultAsync<Footer>(query, model);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating article: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = @"
            UPDATE footer_contact
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


    public async Task<FooterVM?> GetFooterByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM footer_contact WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                FooterVM? model = await connection.QuerySingleOrDefaultAsync<FooterVM>(query, new { Id = id });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Footer>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @$"
                        SELECT
                            id AS ""Id"",
                            name AS ""Name"",
                            value AS ""Value"",
                            created_at AS ""CreatedAt"",
                            updated_at AS ""UpdatedAt"";
                        FROM footer_contact
                        WHERE deleted_at IS NULL
                        LIMIT 10
                        ORDER BY created_at DESC 
                        ;";

            var result = await connection.QueryAsync<Footer>(query);
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

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataFooter(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    footer_contact 
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
                    (LOWER(name) LIKE @SearchValue OR
                    LOWER(value) LIKE @SearchValue)");
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

            query += @" ORDER BY updated_at DESC";

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

    public async Task<List<Footer>> GetListFooterAsync()
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                name AS ""Name"",
                value AS ""Value"",
                created_at AS ""CreatedAt""
            FROM footer_contact
            WHERE deleted_at IS NULL
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var article = await connection.QueryAsync<Footer>(query);
                return article.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Footer: {ex.Message}");
            return null;
        }
    }

    public async Task<Footer?> GetFooterBySlugAsync(string id)
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                name AS ""Name"",
                value AS ""Value"",
                created_at AS ""CreatedAt""
            FROM footer_contact
            WHERE id = @Id
            AND deleted_at IS NULL;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Footer? model = await connection.QuerySingleOrDefaultAsync<Footer>(query, new { Id = id });
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