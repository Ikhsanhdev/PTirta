using Higertech.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Higertech.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetByNameAsync(string nama_produk);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<int> CreateAsync(Product product);
    Task<int> UpdateAsync(Product product);
    Task<int> DeleteAsync(Guid id);
}

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = "SELECT * FROM products WHERE deleted_at IS NULL ORDER BY created_at DESC";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = "SELECT * FROM products WHERE id = @id AND deleted_at IS NULL";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { id = id });
    }

    public async Task<Product?> GetByNameAsync(string nama_produk)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = "SELECT * FROM products WHERE nama_produk = @nama_produk AND deleted_at IS NULL";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { nama_produk = nama_produk });
    }

    public async Task<int> CreateAsync(Product product)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = @"
            INSERT INTO products (id, nama_produk, gambar_url, kategori, created_at, updated_at) 
            VALUES (@id, @nama_produk, @gambar_url, @kategori, @created_at, @updated_at)";

        product.id = Guid.NewGuid();
        product.created_at = DateTime.UtcNow;
        product.updated_at = DateTime.UtcNow;

        return await connection.ExecuteAsync(sql, product);
    }

    public async Task<int> UpdateAsync(Product product)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = @"
            UPDATE products 
            SET nama_produk = @nama_produk, gambar_url = @gambar_url, kategori = @kategori, updated_at = @updated_at
            WHERE id = @id";

        product.updated_at = DateTime.UtcNow;
        return await connection.ExecuteAsync(sql, product);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        string sql = "UPDATE products SET deleted_at = @deleted_at WHERE id = @id";
        return await connection.ExecuteAsync(sql, new { id = id, deleted_at = DateTime.UtcNow });
    }
}
