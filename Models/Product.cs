using System;
using System.Collections.Generic;

namespace Higertech.Models;
public class Product
{
    public Guid id { get; set; }
    public required string nama_produk { get; set; } 
    public required string gambar_url { get; set; }
    public required string kategori { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}



public class ProductViewModel
{
    public Guid id { get; set; }
    public string nama_produk { get; set; }
    public string gambar_url { get; set; }
    public string kategori { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}