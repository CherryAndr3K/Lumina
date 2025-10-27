using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Model
{
    public enum MediaType { Book, Movie, Music }

    public class FavoriteItem
    {
        public string Title { get; set; } = "";
        public MediaType Type { get; set; }
        public string? ImagePath { get; set; }
    }
}

