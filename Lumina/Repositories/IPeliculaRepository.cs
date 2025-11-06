using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Repositories
{
    public interface IPeliculaRepository
    {
        IEnumerable<Pelicula> GetAll();
        Pelicula? GetById(int id);
        int Add(Pelicula pelicula);
        bool Update(Pelicula pelicula);
        bool Delete(int id);
    }
}
