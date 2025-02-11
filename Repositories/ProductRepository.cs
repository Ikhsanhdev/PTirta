using Higertech.Models;
using Higertech.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using Higertech.Models.Datatables;

namespace Higertech.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataProduct(JqueryDataTableRequest request);
    Task<ProductVM?> GetProductByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(ProductVM product);
    Task<bool> DeleteAsync(Guid id);
}

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;
    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<List<Product>> GetAllAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @$"
                    SELECT
                        id AS ""Id"",
                        nama_produk AS ""NamaProduk"",
                        gambar_url AS ""GambarUrl"",
                        kategori AS ""Kategori"",
                        deskripsi AS ""Deskripsi"",
                        created_at AS ""CreatedAt"",
                        updated_at AS ""UpdatedAt""
                    FROM products
                    WHERE deleted_at IS NULL
                    ORDER BY created_at DESC 
                    LIMIT 10;";

            var result = await connection.QueryAsync<Product>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace });
            throw;
        }
    }

    public async Task<AjaxResponse> SaveAsync(ProductVM product)
    {
        AjaxResponse result = new();
        try
        {
            string query;
            string status = "Tambah";

            if (product.id == Guid.Empty)
            {
                query = @"
                    INSERT INTO products 
                    (nama_produk, gambar_url, kategori, deskripsi)
                    VALUES 
                    (@nama_produk, @gambar_url, @kategori, @deskripsi)
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                product.updated_at = DateTime.Now;
                query = @"
                    UPDATE products
                    SET nama_produk = @nama_produk, 
                        gambar_url = @gambar_url, 
                        kategori = @kategori, 
                        deskripsi = @deskripsi,
                        updated_at = @updated_at
                    WHERE id = @id
                    RETURNING *;";
            }

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var data = await connection.ExecuteAsync(query, product);
                result.Code = 200;
                result.Message = $"{status} data berhasil";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = $"Terjadi Kesalahan.\nError: {ex}";
        }
        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string query = @"
            UPDATE products
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
            Log.Error(ex, "Error deleting product: {Message}", ex.Message);
            return false;
        }
    }

    public async Task<ProductVM?> GetProductByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM products WHERE id = @Id;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                ProductVM? product = await connection.QuerySingleOrDefaultAsync<ProductVM>(query, new { Id = id });
                return product;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching product: {Message}", ex.Message);
            return null;
        }
    }

    public async Task<(IReadOnlyList<dynamic>, int)> GetDataProduct(JqueryDataTableRequest request)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            List<dynamic> result = new List<dynamic>();

            var query = @"SELECT * FROM products ";

            var parameters = new DynamicParameters();
            var whereConditions = new List<string>();

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                if (request.SearchValue.Contains('\''))
                {
                    request.SearchValue = request.SearchValue.Replace("'", "''");
                }

                whereConditions.Add(@"
                    (LOWER(nama_produk) LIKE @SearchValue OR
                    LOWER(kategori) LIKE @SearchValue OR
                    LOWER(deskripsi) LIKE @SearchValue)");
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
            Log.Error(ex, "Error getting product data: {Message}", ex.Message);
            throw;
        }
    }
}