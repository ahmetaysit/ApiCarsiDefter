using Core;
using Interfaces.Repository;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.BusinessIntefaces
{
    public interface ICustomerGroupBS : IGenericRepository<CustomerGroup>, IBaseBS
    {
        CustomerGroup Get(int groupId);
        bool Save(CustomerGroup grp);
    }
}
