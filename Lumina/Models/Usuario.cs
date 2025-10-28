using System;
using System.Collections.Generic;

namespace Lumina.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Avatar { get; set; }

    public string Contrasena { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
}
