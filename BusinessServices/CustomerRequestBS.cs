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
    public class CustomerRequestBS : GenericRepositoryBs<ICustomerRequestDAL,CustomerRequest>,ICustomerRequestBS
    {
        ICustomerRequestDAL _dal;
        public CustomerRequestBS(ICustomerRequestDAL dal):base(dal)
        {
            _dal = dal;
        }
    }
}
