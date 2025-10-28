using System;
using System.Collections.Generic;

namespace Lumina.Models;

public partial class Favorito
{
    public int FavoritoId { get; set; }

    public int UsuarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public int ReferenciaId { get; set; }

    public DateTime FechaMarcado { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
