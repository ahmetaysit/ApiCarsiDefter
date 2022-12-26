using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Core;
using Library.Entities;
using Interfaces;
using Library.Enums;

namespace BusinessServices
{
    public class ExcelCustomerBS : IExcelCustomerBS
    {
        ICustomerAccountBS _customerAccountBS;
        ICurrencyBS _currencyBS;
        ICustomerBS _customerBS;
        ITransactionBS _transactionBS;

        public ExcelCustomerBS(ICustomerAccountBS customerAccountBs,ICurrencyBS currencyBs,ICustomerBS customerBs, ITransactionBS transactionBS)
        {
            _customerAccountBS = customerAccountBs;
            _currencyBS = currencyBs;
            _customerBS = customerBs;
            _transactionBS = transactionBS;
        }

        public void CreateCustomers(List<ExcelToCustomerDto> lst)
        {
            List<Currency> currencies = _currencyBS.GetAllCurrencies();

            foreach (ExcelToCustomerDto dto in lst)
            {
                Customer cst = new Customer();
                cst.CustomerName = dto.CustomerName;
                cst.CustomerGroupId = dto.CustomerGroupId;
                cst.Email = dto.Email;
                cst.IsActive = true;
                cst.PhoneNumber = dto.PhoneNumber;
                cst.PoolRate = dto.PoolRate;

                _customerBS.AddCustomer(cst);

                foreach (Currency item in currencies)
                {
                    ExcelCustomerAccountDto accDto = dto.Accounts.Where(x => x.CurrencyId == item.Id).FirstOrDefault();
                    CustomerAccount customerAccount = new CustomerAccount();
                    customerAccount.AccountNo = item.CurrencyCode + " Hesap 001";
                    customerAccount.IsActive = true;
                    customerAccount.CustomerId = cst.Id;
                    customerAccount.Currency = item.Id;

                    _customerAccountBS.CreateNewWithBalance(customerAccount, 0);

                    Transaction tran = new Transaction
                    {
                        CustomerId = cst.Id,
                        CustomerAccountId = customerAccount.Id,
                        TransactionType = (int)TransactionType.ManuelIncome,
                        TransactionAmount = accDto.Amount,
                        TransactionDate = dto.TransactionDate,
                        CurrencyId = item.Id,
                        IsProcessed = true,
                        CurrencyRate = 1,
                        BuyingRate = 1,
                        SellingRate = 1,
                        CreationDate = DateTime.Now,
                        IsShopProfitTaken = false
                    };

                    _transactionBS.Insert(tran);

                }

            }
        }
    }
}
