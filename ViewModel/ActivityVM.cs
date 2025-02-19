using System;
using System.Collections.Generic;

namespace Higertech.ViewModels;

public partial class ActivityVM
{
    public Guid id { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public string? img_url { get; set; }
    public string? client_name { get; set; }
    public DateTime date_activity { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}