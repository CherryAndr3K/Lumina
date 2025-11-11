using System;
using System.Collections.Generic;

namespace Lumina.Models;

public partial class Albumes
{
    public int AlbumesId { get; set; }   
    public string Titulo { get; set; } = null!;
    public string? Artista { get; set; }
    public string? Genero { get; set; }
}
