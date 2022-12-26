using Core;
using Library.Entities;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DALINterfaces
{
    public interface IPoolIncomeFromGroupDAL : IGenericRepository<PoolIncomeFromGroup>, IBaseDAL
    {
        void Add(PoolIncomeFromGroup entity);
    }
}
