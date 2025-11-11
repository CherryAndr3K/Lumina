using System;
using System.Collections.Generic;

namespace Lumina.Models;

public partial class Pelicula
{
    public int PeliculaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Genero { get; set; }

    public int? Anio { get; set; }

    public string? Link { get; set; }

    public string? Imagen { get; set; }
}
