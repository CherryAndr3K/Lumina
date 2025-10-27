using System.Collections.Generic;
using System.Linq;

namespace Lumina.Services
{
    public enum MediaType { Book, Movie, Music }

    public class FavoriteItem
    {
        public string Title { get; set; } = "";
        public MediaType Type { get; set; }
        public string? ImagePath { get; set; }
    }

    public static class FavoritesStore
    {
        public static readonly List<FavoriteItem> Items = new();

        public static bool IsFavorite(string title, MediaType type)
            => Items.Any(x => x.Type == type && x.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));

        // Si existe lo quita; si no, lo agrega. Devuelve true si quedó agregado.
        public static bool Toggle(string title, MediaType type, string? imagePath = null)
        {
            var it = Items.FirstOrDefault(x => x.Type == type && x.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));
            if (it != null) { Items.Remove(it); return false; }
            Items.Add(new FavoriteItem { Title = title, Type = type, ImagePath = imagePath });
            return true;
        }
    }
}
