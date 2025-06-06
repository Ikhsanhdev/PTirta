using System;
using System.Collections.Generic;

namespace TirtaRK.ViewModels;

public partial class ProjectVM
{
    public Guid id { get; set; }
    public string? title { get; set; }
    public string? img_url { get; set; }
    public string? slug { get; set; }
    public DateTime? create_at { get; set; }
    public DateTime? updated_at { get; set; }
}