using System;
using System.Collections.Generic;

namespace TirtaRK.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? Username { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}
