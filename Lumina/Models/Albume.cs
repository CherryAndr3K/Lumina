using System;
using System.Collections.Generic;

namespace Lumina.Models;

public partial class Albume
{
    public int AlbumId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Artista { get; set; }

    public string? Genero { get; set; }
}
