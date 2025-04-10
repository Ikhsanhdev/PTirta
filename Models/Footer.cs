using System;
using System.Collections.Generic;

namespace Higertech.Models;

public partial class Footer
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}