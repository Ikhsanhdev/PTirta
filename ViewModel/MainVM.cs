using System;
using System.Collections.Generic;
using Higertech.Models;

namespace Higertech.ViewModels
{
    public class MainVM
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string img_url { get; set; }
        public string category { get; set; }
        public string hide { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}

