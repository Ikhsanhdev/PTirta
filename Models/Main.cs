using System;
using System.Collections.Generic;

using System;

namespace Higertech.Models
{
    public class Main
    {
        public Guid Slug { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Img_Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MainViewModel
    {
        public List<Main> Mains { get; set; } = new List<Main>();
        public List<Article> Articles { get; set; } = new List<Article>();
    }
}

