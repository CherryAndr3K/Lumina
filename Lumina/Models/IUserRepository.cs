using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Models
{
    public interface IUserRepository
    {
        bool AutenticateUser(NetworkCredential credential);
        void Add(UserModel useModel);
        void Update(UserModel useModel);
        void Delete(string username);
        UserModel GetByUsername(string username);


    }
}
