using Core;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICustomerBS : IBaseBS
    {
        List<Customer> GetCustomers();
        List<Customer> GetCustomersForJustBalance();
        Customer Get(int customerId);
        List<CustomerAccountDto> GetCustomerAccounts(int customerId);
        List<CustomerAccountDtoEx> GetAllCustomerAccounts();
        bool SaveCustomer(Customer customer);

        bool CreateCustomerWithBalance(AddCustomerSummaryDto customer);
        List<CustomerGroup> GetCustomerGroups();
        Customer AddCustomer(Customer customer);
        bool ApproveCustomerRequest(int id);
        bool RejectCustomerRequest(int id);
    }
}
