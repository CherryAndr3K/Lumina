using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Views;
using Lumina.Models;

namespace Lumina.Repositories
{
    public interface IFavoritoRepository
    {
        IEnumerable<FavoritoView> GetAllViews();
        int Add(Favorito favorito);     // Favorito es tu entidad EF (Lumina.Models)
        bool Delete(int id);
    }
}
