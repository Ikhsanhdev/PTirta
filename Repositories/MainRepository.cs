using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IMainRepository
{
    Task<List<Main>> GetAllAsync();
    // Task<(IReadOnlyList<dynamic>, int)> GetDataMain(JqueryDataTableRequest request);
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
                    slug,
                    title,
                    description,
                    img_url,
                    created_at,
                    update_at
                FROM home
                WHERE img_url IS NOT NULL
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

    // public async Task<(IReadOnlyList<dynamic>, int)> GetDataMain(JqueryDataTableRequest request)
    // {
    //     try
    //     {
    //         using var connection = new NpgsqlConnection(_connectionString);
    //         List<dynamic> result = new List<dynamic>();

    //         var query = @"SELECT * FROM home ";

    //         var parameters = new DynamicParameters();
    //         var whereConditions = new List<string>();

    //         if (!string.IsNullOrEmpty(request.SearchValue))
    //         {
    //             if (request.SearchValue.Contains('\''))
    //             {
    //                 request.SearchValue = request.SearchValue.Replace("'", "''");
    //             }

    //             whereConditions.Add(@"
    //                 (LOWER(title) LIKE @SearchValue OR
    //                 LOWER(description) LIKE @SearchValue)");
    //             parameters.Add("@SearchValue", "%" + request.SearchValue.ToLower() + "%");
    //         }
            
    //         // whereConditions.Add(@" deleted_at IS NULL");

    //         var whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

    //         query += whereClause;

    //         int total = 0;
    //         var sql_count = $"SELECT COUNT(*) FROM ({query}) as total";
    //         total = connection.ExecuteScalar<int>(sql_count, parameters);

    //         query += @" 
    //         OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;";
    //         parameters.Add("@Skip", request.Skip);
    //         parameters.Add("@PageSize", request.PageSize);

    //         result = (await connection.QueryAsync<dynamic>(query, parameters)).ToList();

    //         return (result, total);
    //     }
    //     catch (Exception ex)
    //     {
    //         Log.Error(ex, "Error getting mains data: {Message}", ex.Message);
    //         throw;
    //     }
    // }
}