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
    public interface ICustomerGroupDAL : IGenericRepository<CustomerGroup>, IBaseDAL
    {
        CustomerGroup Get(int groupId);
        bool Save(CustomerGroup grp);
    }
}
