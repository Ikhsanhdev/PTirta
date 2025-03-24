using System;
using System.Collections.Generic;

using System;

namespace Higertech.Models
{
    public class Main
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Img_Url { get; set; } = string.Empty;
        public string Category { get; set; }
        public string Hide { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class MainViewModel
    {
        public List<Main> Mains { get; set; } = new List<Main>();
        public Main About { get; set; } = new();
        public Main Profile { get; set; } = new();
        public Main Testimoni { get; set; } = new();
        public List<Main> FAQ { get; set; } = new();
        public List<Main> Vision { get; set; } = new();
        public List<Main> Mission { get; set; } = new();
       
        public List<Main> Posters { get; set; } = new();
        public List<Main> Tombol { get; set; } = new();
        public List<Main> Layanan { get; set; } = new();
        public List<Main> Klien { get; set; } = new();
        public List<Work> Works { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Gallery> Galleries { get; set; } = new();
    }
}

