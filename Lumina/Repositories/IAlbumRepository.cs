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
        IEnumerable<Albume> GetAll();
        Albume? GetById(int id);
        int Add(Albume album);
        bool Update(Albume album);
        bool Delete(int id);
    }
}
