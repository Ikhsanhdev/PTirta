using TirtaRK.Models;
using TirtaRK.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using TirtaRK.Models.Datatables;

namespace TirtaRK.Repositories;

public interface IMainRepository
{
    Task<List<Main>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataMain(JqueryDataTableRequest request);
    Task<MainVM> GetMainByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(MainVM main);
    Task<Main?> UpdateMainAsync(Main main);
    Task<bool> DeleteMainAsync(Guid id);
    Task<AjaxResponse> ToggleHideAsync(Guid id);
    Task<dynamic> GetFooterAsync();
}

public class MainRepository : IMainRepository
{
    private readonly string _connectionString;
    public MainRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<List<Main>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"
                SELECT
                    id AS ""Id"",
                    title AS ""Title"",
                    description AS ""Description"",
                    img_url AS ""Img_Url"",
                    category AS ""Category"",
                     hide AS ""Hide"",
                    created_at AS ""CreatedAt"",
                    updated_at AS ""UpdatedAt""
                FROM home
                WHERE deleted_at IS NULL
                AND hide = 'on'  -- Only show items where hide is 'on'
                ORDER BY created_at DESC;";

            var result = await connection.QueryAsync<Main>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching main data: {@ExceptionDetails}", 
                new { ex.Message, ex.StackTrace });
            throw;
        }
    }

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataMain(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = @"SELECT * FROM home ";

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
                    LOWER(category) LIKE @SearchValue OR
                    LOWER(description) LIKE @SearchValue)");
                parameters.Add("@SearchValue", "%" + request.SearchValue.ToLower() + "%");
            }
            
            whereConditions.Add(@" deleted_at IS NULL");

            var whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

            query += whereClause;

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
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting home data: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<MainVM> GetMainByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM home WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                MainVM? main = await connection.QuerySingleOrDefaultAsync<MainVM>(query, new { Id = id });
                return main;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching main: {ex.Message}");
            return null;
        }
    }

    public async Task<AjaxResponse> SaveAsync(MainVM main)
    {
        AjaxResponse result = new();
        try
        {
            string query;
            string status = "Tambah";

            if (main.id == Guid.Empty)
            {
                main.id = Guid.NewGuid();
                main.created_at = DateTime.UtcNow;
                query = @"
                    INSERT INTO home 
                    (id, title, img_url, category, description, hide, created_at)
                    VALUES 
                    (@id, @title, @img_url, @category, @description, @hide,@created_at)
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                query = @"
                    UPDATE home
                    SET title = @title, 
                        img_url = @img_url, 
                        category = @category, 
                        description = @description,
                        hide = @hide,
                        updated_at = @updated_at
                    WHERE id = @id AND deleted_at IS NULL
                    RETURNING *;";

                main.updated_at = DateTime.UtcNow;
            }

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var data = await connection.QuerySingleOrDefaultAsync(query, main);
                
                if (data != null)
                {
                    result.Code = 200;
                    result.Message = $"Berhasil {status} data";
                }
                else
                {
                    result.Code = 404;
                    result.Message = "Data tidak ditemukan";
                }
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = $"Terjadi Kesalahan.\nError: {ex.Message}";
            Log.Error(ex, "Error in SaveAsync: {@ExceptionDetails}", 
                new { main.id, main.title, Error = ex.Message });
        }
        return result;
    }

    public async Task<Main?> UpdateMainAsync(Main main)
    {
        const string query = @"
            UPDATE home
            SET title = @title, 
                description = @description, 
                img_url = @img_url, 
                category = @category,
                hide = @hide,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Main>(query, main);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating main: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteMainAsync(Guid id)
    {
        const string query = @"
            UPDATE home
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
            Console.WriteLine($"Error deleting home: {ex.Message}");
            return false;
        }
    }

    public async Task<AjaxResponse> ToggleHideAsync(Guid id)
    {
        AjaxResponse result = new();
        const string getCurrentState = "SELECT hide FROM home WHERE id = @Id AND deleted_at IS NULL;";
        const string updateQuery = @"
            UPDATE home 
            SET hide = @NewState,
                updated_at = @UpdatedAt
            WHERE id = @Id AND deleted_at IS NULL
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // Get current state
                var currentState = await connection.QuerySingleOrDefaultAsync<string>(getCurrentState, new { Id = id });
                if (currentState == null)
                {
                    result.Code = 404;
                    result.Message = "Data tidak ditemukan";
                    return result;
                }

                // Toggle state
                var newState = currentState == "on" ? "off" : "on";
                
                var data = await connection.QuerySingleOrDefaultAsync(updateQuery, new { 
                    Id = id, 
                    NewState = newState,
                    UpdatedAt = DateTime.UtcNow
                });

                if (data != null)
                {
                    result.Code = 200;
                    result.Message = $"Status berhasil diubah menjadi {newState}";
                }
                else
                {
                    result.Code = 500;
                    result.Message = "Gagal mengubah status";
                }
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = $"Terjadi Kesalahan.\nError: {ex.Message}";
            Log.Error(ex, "Error in ToggleHideAsync: {@ExceptionDetails}", 
                new { Id = id, Error = ex.Message });
        }
        return result;
    }
      public async Task<dynamic> GetFooterAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
           var result = await connection.QueryAsync<dynamic>("SELECT name, value FROM footer_contact WHERE deleted_at IS NULL;");

            var contactData = new Dictionary<string, string>();
        var socialLinks = new Dictionary<string, string>();

        foreach (var item in result)
        {
            string name = item.name.ToLower();
            string value = item.value;

            // Jika value adalah URL, masukkan ke socialLinks
            if (value.StartsWith("http"))
            {
                socialLinks[name] = value;
            }
            else
            {
                contactData[name] = value;
            }
        }

        var response = new
        {
            contact = contactData,
            social_links = socialLinks
        };

        return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching footer data: {@ExceptionDetails}",
                new { ex.Message, ex.StackTrace });
            throw;

        }
    }

}