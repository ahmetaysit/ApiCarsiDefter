using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Library.Entities;
using Interfaces.DALINterfaces;
using BusinessServices.Repository;

namespace BusinessServices
{
    public class CustomerGroupBS : GenericRepositoryBs<ICustomerGroupDAL,CustomerGroup>,ICustomerGroupBS
    {
        ICustomerGroupDAL _customerGroupDal;
        public CustomerGroupBS(ICustomerGroupDAL customerGroupDal):base(customerGroupDal)
        {
            _customerGroupDal = customerGroupDal;
        }
        public CustomerGroup Get(int groupId)
        {
            return _customerGroupDal.Get(groupId);
        }

        public bool Save(CustomerGroup grp)
        {
            return _customerGroupDal.Save(grp);
        }
    }
}
