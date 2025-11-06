using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Repositories
{
    public interface ILibroRepository
    {
        IEnumerable<Libro> GetAll();
        Libro? GetById(int id);
        int Add(Libro libro);
        bool Update(Libro libro);
        bool Delete(int id);
    }
}
