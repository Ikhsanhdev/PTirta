using System;
using System.Collections.Generic;

namespace Higertech.ViewModels;

public partial class ProductVM
{
    public Guid id { get; set; }
    public string? nama_produk { get; set; }
    public string? gambar_url { get; set; }
    public string? kategori { get; set; }
    public string? deskripsi { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}