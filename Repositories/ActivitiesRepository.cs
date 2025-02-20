using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IActivitiesRepository
{
    Task<List<ActivityModel>> GetListActivityAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataActivity(JqueryDataTableRequest request);
    Task<ActivityVM?> GetActivityByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(ActivityVM article);
    Task<bool> DeleteAsync(Guid id);
}

public class ActivitiesRepository : IActivitiesRepository
{
    private readonly string _connectionString;
    public ActivitiesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<AjaxResponse> SaveAsync(ActivityVM model)
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
                    activities ( title, description, client_name,date_project, img_url,date_activity)
                    VALUES ( @title,@description,@client_name,@date_project, @img_url,@date_activity)
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                model.updated_at = DateTime.Now;
                query = @"
                        UPDATE activities
                        SET title = @title ,description = @description,client_name = @client_name,date_project = @date_project,img_url = @img_url, updated_at = @updated_at, date_activity = @date_activity
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
    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = @"
            UPDATE activities
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


    public async Task<ActivityVM?> GetActivityByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM activities WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                ActivityVM? model = await connection.QuerySingleOrDefaultAsync<ActivityVM>(query, new { Id = id });
                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article: {ex.Message}");
            return null;
        }
    }

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataActivity(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = $@"SELECT 
                    *
                    FROM 
                    activities 
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
                    (LOWER(title) LIKE @SearchValue)");
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

    public async Task<List<ActivityModel>> GetListActivityAsync()
    {
        const string query = @"
            SELECT 
                id AS ""Id"",
                title AS ""Title"",
                description AS ""Description"",
                img_url AS ""Image"",
                client_name AS ""ClientName"",
                created_at AS ""CreatedAt"",
                date_activity AS ""DateActivity"",
                date_project AS ""DateProject""
            FROM activities
            WHERE deleted_at IS NULL
            ORDER BY created_at DESC;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var model = await connection.QueryAsync<ActivityModel>(query);
                return model.ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching project: {ex.Message}");
            return null;
        }
    }

}