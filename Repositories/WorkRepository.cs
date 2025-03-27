using TirtaRK.Models;
using TirtaRK.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using TirtaRK.Models.Datatables;

namespace TirtaRK.Repositories;

public interface IWorkRepository
{

    Task<List<Work>> GetListWorkAsync();
    Task<Work?> GetWorkBySlugAsync(string slug);
    Task<List<Work>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataWork(JqueryDataTableRequest request);
    Task<WorkVM> GetWorkByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(WorkVM work);
    Task<Work?> UpdateWorkAsync(Work work);
    Task<bool> DeleteWorkAsync(Guid id);
}

public class WorkRepository : IWorkRepository
{
    private readonly string _connectionString;
    public WorkRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<AjaxResponse> SaveAsync(WorkVM work)
    {
        AjaxResponse result = new();
        try
        {
            string query = "";
            string status = "Tambah";

            if (work.id == Guid.Empty)
            {
                work.author = "Admin";
                query = @"
                    INSERT INTO 
                    works ( title, description, author, img_url, category, slug)
                    VALUES ( @title, @description, @author, @img_url, @category , LOWER(REPLACE(@title, ' ', '-')))
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                work.updated_at = DateTime.Now;
                query = @"
                        UPDATE works
                        SET title = @title, description = @description, author = @author, img_url = @img_url, updated_at = @updated_at, category = @category , slug = LOWER(REPLACE(@title, ' ', '-'))
                        WHERE id = @Id
                        RETURNING *;";

            }


            using (var connection = new NpgsqlConnection(_connectionString))
            {

                var data = await connection.ExecuteAsync(query, work);

                result.Code = 200;
                result.Message = $"{status} data berhasil";
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving works: {ex.Message}");
            result.Code = 500;
            result.Message = "Terjadi Kesalahan saat menyimpan data";
        }
        return result;
    }

    public async Task<Work?> UpdateWorkAsync(Work work)
    {
        const string query = @"
            UPDATE works
            SET title = @Title, deskripsi = @Deskripsi, penulis = @Penulis, gambar_url = @GambarUrl, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Work>(query, work);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating works: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteWorkAsync(Guid id)
    {
        const string query = @"
            UPDATE works
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
            Console.WriteLine($"Error deleting works: {ex.Message}");
            return false;
        }
    }


    public async Task<WorkVM> GetWorkByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM works WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var work = await connection.QuerySingleOrDefaultAsync<WorkVM>(query, new { Id = id });
                return work;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching work: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Work>> GetAllAsync()
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
                            img_url AS ""Image"",
                            created_at AS ""CreatedAt"",
                            updated_at AS ""UpdatedAt"";
                        FROM works
                        LIMIT 10
                        ORDER BY created_at DESC 
                        ;";

            var result = await connection.QueryAsync<Work>(query);
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

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataWork(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    works 
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

            query += @" ORDER BY updated_at DESC";
            
            int total = 0;
            var sql_count = $"SELECT COUNT(*) FROM ({query}) as total";

            query += @" 
                OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;";
            parameters.Add("@Skip", request.Skip);
            parameters.Add("@PageSize", request.PageSize);

            total = connection.ExecuteScalar<int>(sql_count, parameters);
            
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

    public async Task<List<Work>> GetListWorkAsync()
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
            FROM works
            WHERE deleted_at IS NULL 
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var work = await connection.QueryAsync<Work>(query);
                return work.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching work: {ex.Message}");
            return null;
        }
    }

    public async Task<Work?> GetWorkBySlugAsync(string slug)
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
            FROM works
            WHERE slug = @Slug
            AND deleted_at IS NULL;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Work? model = await connection.QuerySingleOrDefaultAsync<Work>(query, new { Slug = slug });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching work: {ex.Message}");
            return null;
        }
    }
}