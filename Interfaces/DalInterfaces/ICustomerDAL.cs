using Core;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DALINterfaces
{
    public interface ICustomerDAL : IBaseDAL
    {
        List<Customer> GetCustomers();
        List<Customer> GetCustomersForJustBalance();
        Customer Get(int customerId);
        List<CustomerAccountDto> GetCustomerAccounts(int customerId);
        List<CustomerAccountDtoEx> GetAllCustomerAccounts();
        Customer SaveCustomer(Customer customer);
        List<CustomerGroup> GetCustomerGroups();
    }
}
