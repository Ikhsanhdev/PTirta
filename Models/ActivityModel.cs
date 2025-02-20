using System;
using System.Collections.Generic;

namespace Higertech.Models;

// Do not Rename This Name Model because Activiy will distract another package on built in function dotnet
public partial class ActivityModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? ClientName { get; set; }
    public DateTime DateActivity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}