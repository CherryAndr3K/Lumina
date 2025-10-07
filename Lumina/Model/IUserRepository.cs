using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Model
{
    internal class IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void Add(UserModel useModel);
        void Update(UserModel useModel);
        void Delete(UserModel useModel);
    }
}
