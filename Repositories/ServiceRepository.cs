using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IServiceRepository
{

    Task<List<Service>> GetListServiceAsync();
    Task<Service?> GetServiceBySlugAsync(string slug);
    Task<List<Service>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataService(JqueryDataTableRequest request);
    Task<ServiceVM> GetServiceByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(ServiceVM service);
    Task<Service?> UpdateServiceAsync(Service service);
    Task<bool> DeleteServiceAsync(Guid id);
}

public class ServiceRepository : IServiceRepository
{
    private readonly string _connectionString;
    public ServiceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<AjaxResponse> SaveAsync(ServiceVM service)
    {
        AjaxResponse result = new();
        try
        {
            string query = "";
            string status = "Tambah";

            if (service.id == Guid.Empty)
            {
                service.author = "Admin";
                query = @"
                    INSERT INTO 
                    services ( title, description, author, img_url, category, slug)
                    VALUES ( @title, @description, @author, @img_url, @category , LOWER(REPLACE(@title, ' ', '-')))
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                service.updated_at = DateTime.Now;
                query = @"
                        UPDATE services
                        SET title = @title, description = @description, author = @author, img_url = @img_url, updated_at = @updated_at, category = @category , slug = LOWER(REPLACE(@title, ' ', '-'))
                        WHERE id = @Id
                        RETURNING *;";

            }


            using (var connection = new NpgsqlConnection(_connectionString))
            {

                var data = await connection.ExecuteAsync(query, service);

                result.Code = 200;
                result.Message = $"{status} data berhasil";
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving services: {ex.Message}");
            result.Code = 500;
            result.Message = "Terjadi Kesalahan saat menyimpan data";
        }
        return result;
    }

    public async Task<Service?> UpdateServiceAsync(Service service)
    {
        const string query = @"
            UPDATE services
            SET title = @Title, deskripsi = @Deskripsi, penulis = @Penulis, gambar_url = @GambarUrl, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Service>(query, service);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating services: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteServiceAsync(Guid id)
    {
        const string query = @"
            UPDATE services
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
            Console.WriteLine($"Error deleting services: {ex.Message}");
            return false;
        }
    }


    public async Task<ServiceVM> GetServiceByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM services WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var service = await connection.QuerySingleOrDefaultAsync<ServiceVM>(query, new { Id = id });
                return service;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching service: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Service>> GetAllAsync()
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
                        FROM services
                        LIMIT 10
                        ORDER BY created_at DESC 
                        ;";

            var result = await connection.QueryAsync<Service>(query);
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

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataService(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    services 
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

    public async Task<List<Service>> GetListServiceAsync()
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
            FROM services
            WHERE deleted_at IS NULL 
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var service = await connection.QueryAsync<Service>(query);
                return service.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching service: {ex.Message}");
            return null;
        }
    }

    public async Task<Service?> GetServiceBySlugAsync(string slug)
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
            FROM services
            WHERE slug = @Slug
            AND deleted_at IS NULL;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Service? model = await connection.QuerySingleOrDefaultAsync<Service>(query, new { Slug = slug });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching service: {ex.Message}");
            return null;
        }
    }
}