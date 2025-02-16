using System;
using System.Collections.Generic;

namespace Higertech.Models;

public partial class Project
{
    public Guid Id { get; set; }
    public string? Title { get; set; } 
    public string? Image { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}