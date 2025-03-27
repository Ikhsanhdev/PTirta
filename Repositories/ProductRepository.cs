using TirtaRK.Models;
using TirtaRK.ViewModels;
using Dapper;
using Npgsql;
using Serilog;
using TirtaRK.Models.Datatables;

namespace TirtaRK.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<(IReadOnlyList<dynamic>, int)> GetDataProduct(JqueryDataTableRequest request);
    Task<ProductVM?> GetProductByIdAsync(Guid id);
    Task<AjaxResponse> SaveAsync(ProductVM product);
    Task<Product?> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(Guid id);
    Task<bool> IsProductNameExistsAsync(string productName, Guid? excludeId = null);
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
                    ORDER BY created_at DESC;";

            var result = await connection.QueryAsync<Product>(query);
            return result.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "General Exception: {@ExceptionDetails}", new { ex.Message, ex.StackTrace });
            throw;
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
            Console.WriteLine($"Error fetching product: {ex.Message}");
            return null;
        }
    }

    // public async Task<AjaxResponse> SaveAsync(ProductVM product)
    // {
    //     AjaxResponse result = new();
    //     try
    //     {
    //         string query;
    //         string status = "Tambah";

    //         if (product.id == Guid.Empty)
    //         {
    //             product.created_at = DateTime.Now;
    //             query = @"
    //                 INSERT INTO products 
    //                 (nama_produk, gambar_url, kategori, deskripsi, created_at)
    //                 VALUES 
    //                 (@nama_produk, @gambar_url, @kategori, @deskripsi, @created_at)
    //                 RETURNING *;";
    //         }
    //         else
    //         {
    //             status = "Memperbarui";
    //             product.updated_at = DateTime.Now;
    //             query = @"
    //                 UPDATE products
    //                 SET nama_produk = @nama_produk, 
    //                     gambar_url = @gambar_url, 
    //                     kategori = @kategori, 
    //                     deskripsi = @deskripsi,
    //                     updated_at = @updated_at
    //                 WHERE id = @Id
    //                 RETURNING *;";
    //         }

    //         using (var connection = new NpgsqlConnection(_connectionString))
    //         {
    //             var data = await connection.ExecuteAsync(query, product);
    //             result.Code = 200;
    //             result.Message = $"{status} data berhasil";
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         result.Code = 500;
    //         result.Message = $"Terjadi Kesalahan.\nError: {ex}";
    //     }
    //     return result;
    // }

    public async Task<AjaxResponse> SaveAsync(ProductVM product)
    {
        AjaxResponse result = new();
        try
        {
            // Check for duplicate name first
            var isDuplicate = await IsProductNameExistsAsync(product.nama_produk, product.id);
            if (isDuplicate)
            {
                result.Code = 400;
                result.Message = "Nama produk sudah digunakan. Silakan gunakan nama lain.";
                return result;
            }

            string query;
            string status = "Tambah";

            if (product.id == Guid.Empty)
            {
                product.created_at = DateTime.Now;
                query = @"
                    INSERT INTO products 
                    (nama_produk, gambar_url, kategori, deskripsi, created_at)
                    VALUES 
                    (UPPER(@nama_produk), @gambar_url, @kategori, @deskripsi, @created_at)
                    RETURNING *;";
            }
            else
            {
                status = "Memperbarui";
                product.updated_at = DateTime.Now;
                query = @"
                    UPDATE products
                    SET nama_produk = UPPER(@nama_produk), 
                        gambar_url = @gambar_url, 
                        kategori = @kategori, 
                        deskripsi = @deskripsi,
                        updated_at = @updated_at
                    WHERE id = @Id
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

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        const string query = @"
            UPDATE products
                    SET nama_produk = @nama_produk, 
                        gambar_url = @gambar_url, 
                        kategori = @kategori, 
                        deskripsi = @deskripsi,
                        updated_at = @updated_at
                    WHERE id = @Id
            RETURNING *;";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Product>(query, product);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating product: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteProductAsync(Guid id)
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
            Console.WriteLine($"Error deleting product: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsProductNameExistsAsync(string productName, Guid? excludeId = null)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"
                SELECT COUNT(*)
                FROM products 
                WHERE LOWER(nama_produk) = LOWER(@productName) 
                AND deleted_at IS NULL";

            var parameters = new DynamicParameters();
            parameters.Add("@productName", productName);

            if (excludeId.HasValue)
            {
                query += " AND id != @excludeId";
                parameters.Add("@excludeId", excludeId.Value);
            }

            var count = await connection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error checking product name existence: {Message}", ex.Message);
            throw;
        }
    }
}