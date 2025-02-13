using System;
using System.Collections.Generic;
using Higertech.Models;

namespace Higertech.ViewModels
{
    public class MainVM
    {
        public Guid slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string img_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime update_at { get; set; }
    }
}

