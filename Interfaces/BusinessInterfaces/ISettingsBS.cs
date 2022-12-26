using Core;
using Interfaces.Repository;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.BusinessInterfaces
{
    public interface ISettingsBS : IGenericRepository<Settings>, IBaseBS
    {
        Settings Get(string key);
    }
}
