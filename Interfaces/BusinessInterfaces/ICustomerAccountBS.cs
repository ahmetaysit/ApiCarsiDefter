using Core;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Interfaces.Repository;
using Library.Requests;

namespace Interfaces.BusinessIntefaces
{
    public interface ICustomerAccountBS : IGenericRepository<CustomerAccount>, IBaseBS
    {
        bool ExchangeTransfer(CustomerExchangeTransferDto customerAccountDto);
        bool MakeMonthEndProcess(DateTime endOfMonth);
        void ResetAllAccountBalances();
        void DeleteAllAccounts();
        CustomerAccount CreateNewWithBalance(CustomerAccount customerAccount, decimal amount);

        object GenerateEndOfMonthExcel(GenerateEndOfMonthExcelRequest request);
    }
}
