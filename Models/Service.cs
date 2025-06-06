using System;
using System.Collections.Generic;

namespace TirtaRK.Models;

public partial class Service
{
    public Guid Id { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; } 
    public string? Image { get; set; }
    public string? Category { get; set; } 
    public string? Author { get; set; } 
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
}