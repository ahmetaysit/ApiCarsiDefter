using Interfaces.Repository;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IUserService : IGenericRepository<User>
    {
        User Authenticate(string username, string password);
        User Register(string username, string password);
        //IEnumerable<User> GetAll();

        bool IsUserExists(string username);
    }
}
