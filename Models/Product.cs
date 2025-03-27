using System;
using System.Collections.Generic;

namespace TirtaRK.Models;

public partial class Product
{
    public Guid Id { get; set; }
    public string NamaProduk { get; set; } = null!;
    public string? GambarUrl { get; set; }
    public string? Kategori { get; set; }
    public string? Deskripsi { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
