using TirtaRK.Models;
using TirtaRK.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using TirtaRK.Models.Datatables;

namespace TirtaRK.Repositories;

public interface IGalleryRepository
{

    Task<List<Gallery>> GetListGalleryAsync();
    Task<Gallery?> GetGalleryBySlugAsync(string slug);
    Task<List<Gallery>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataGallery(JqueryDataTableRequest request);
    Task<GalleryVM> GetGalleryByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(GalleryVM gallery);
    Task<Gallery?> UpdateGalleryAsync(Gallery gallery);
    Task<bool> DeleteGalleryAsync(Guid id);
}

public class GalleryRepository : IGalleryRepository
{
    private readonly string _connectionString;
    public GalleryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<AjaxResponse> SaveAsync(GalleryVM gallery)
    {
        AjaxResponse result = new();
        try
        {
            string query = "";
            string status = "Tambah";

            if (gallery.id == Guid.Empty)
            {
                query = @"
                    INSERT INTO 
                    gallerys ( title, img_url,  slug)
                    VALUES ( @title,  @img_url, LOWER(REPLACE(@title, ' ', '-')))
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                gallery.updated_at = DateTime.Now;
                query = @"
                        UPDATE gallerys
                        SET title = @title,  img_url = @img_url, updated_at = @updated_at, slug = LOWER(REPLACE(@title, ' ', '-'))
                        WHERE id = @Id
                        RETURNING *;";

            }


            using (var connection = new NpgsqlConnection(_connectionString))
            {

                var data = await connection.ExecuteAsync(query, gallery);

                result.Code = 200;
                result.Message = $"{status} data berhasil";
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving gallerys: {ex.Message}");
            result.Code = 500;
            result.Message = "Terjadi Kesalahan saat menyimpan data";
        }
        return result;
    }

    public async Task<Gallery?> UpdateGalleryAsync(Gallery gallery)
    {
        const string query = @"
            UPDATE gallerys
            SET title = @Title, gambar_url = @GambarUrl, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Gallery>(query, gallery);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating gallerys: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteGalleryAsync(Guid id)
    {
        const string query = @"
            UPDATE gallerys
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
            Console.WriteLine($"Error deleting gallerys: {ex.Message}");
            return false;
        }
    }


    public async Task<GalleryVM> GetGalleryByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM gallerys WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var gallery = await connection.QuerySingleOrDefaultAsync<GalleryVM>(query, new { Id = id });
                return gallery;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching gallery: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Gallery>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @$"
                        SELECT
                            id AS ""Id"",
                            title AS ""Title"",
                            img_url AS ""Image"",
                            created_at AS ""CreatedAt"",
                            updated_at AS ""UpdatedAt"";
                        FROM gallerys
                        LIMIT 10
                        ORDER BY created_at DESC 
                        ;";

            var result = await connection.QueryAsync<Gallery>(query);
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

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataGallery(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    gallerys 
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

    public async Task<List<Gallery>> GetListGalleryAsync()
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                title AS ""Title"",
                img_url AS ""Image"",
                created_at AS ""CreatedAt"",
                slug AS ""Slug""
            FROM gallerys
            WHERE deleted_at IS NULL 
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var gallery = await connection.QueryAsync<Gallery>(query);
                return gallery.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching gallery: {ex.Message}");
            return null;
        }
    }

    public async Task<Gallery?> GetGalleryBySlugAsync(string slug)
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                title AS ""Title"",
                img_url AS ""Image"",
                created_at AS ""CreatedAt""
            FROM gallerys
            WHERE slug = @Slug
            AND deleted_at IS NULL;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Gallery? model = await connection.QuerySingleOrDefaultAsync<Gallery>(query, new { Slug = slug });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching gallery: {ex.Message}");
            return null;
        }
    }

 
}