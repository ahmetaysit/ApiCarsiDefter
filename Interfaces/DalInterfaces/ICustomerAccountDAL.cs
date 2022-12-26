using Core;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Repository;

namespace Interfaces.DALINterfaces
{
    public interface ICustomerAccountDAL :IGenericRepository<CustomerAccount>,  IBaseDAL
    {
        bool ChangeBalance(int accountId, decimal amount);
        CustomerAccount CreateNewWithBalance(CustomerAccount customerAccount, decimal amount);
        bool DeleteAccount(int customerAccountId, bool isAvoidSave);
        CustomerAccountBalance GetBalance(int customerAccountId);
        void ResetAllAccountBalances();
        void DeleteAllAccounts();
    }
}
