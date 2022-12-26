using Core;
using Interfaces.Repository;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DALINterfaces
{
    public interface ISettingsDAL :IGenericRepository<Settings>, IBaseDAL
    {
        Settings Get(string key);
    }
}
