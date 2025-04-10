using System;
using System.Collections.Generic;

namespace Higertech.ViewModels;

public partial class FooterVM
{
    public Guid id { get; set; }
    public string? name { get; set; }
    public string? value { get; set; }
    public DateTime? create_at { get; set; }
    public DateTime? updated_at { get; set; }
}