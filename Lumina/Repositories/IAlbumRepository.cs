using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Models;

namespace Lumina.Repositories
{
    public interface IAlbumRepository
    {
        IEnumerable<Albumes> GetAll();
        Albumes? GetById(int id);
        int Add(Albumes album);
        bool Update(Albumes album);
        bool Delete(int id);
    }
}
