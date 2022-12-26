using Interfaces;
using Interfaces.BusinessIntefaces;
using Interfaces.BusinessInterfaces;
using Library.Dto;
using Library.Entities;
using Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServices
{
    public class ExcelTransactionBS : IExcelTransactionBS
    {
        ICurrencyBS _currencyBS;
        ICustomerAccountBS _customerAccountBS;
        ICustomerBS _customerBS;
        ITransactionBS _transactionBS;

        public ExcelTransactionBS(
            ICurrencyBS currencyBS,
            ICustomerAccountBS customerAccountBS,
            ICustomerBS customerBS,
            ITransactionBS transactionBS
            )
        {
            _currencyBS = currencyBS;
            _customerAccountBS = customerAccountBS;
            _customerBS = customerBS;
            _transactionBS = transactionBS;
        }
        public void AddAllData(List<ExcelToTransactionDto> dtos)
        {
            var allCustomer = _customerBS.GetCustomers();
            var allCurrencies = _currencyBS.GetAllCurrencies();
            //var allAccounts = _customerAccountBS
            foreach (var item in dtos)
            {

                if (item.TransactionType == "GunlukKar")
                {
                    BulkTransactionDto bulkTransactionDto = new BulkTransactionDto
                    {
                        BuyingRate = Convert.ToDecimal(item.BuyingRate),
                        SellingRate = Convert.ToDecimal(item.SellingRate),
                        TransactionDate = item.TransactionDate,
                        SelectedCustomer = allCustomer,
                        SelectedCurrencies = allCurrencies,
                    };
                    _transactionBS.AddBulk(bulkTransactionDto);
                }
                else
                {
                    CustomerExchangeTransferDto customerAccountDto = GenerateDto(allCurrencies, item, item.TransactionType);

                    _customerAccountBS.ExchangeTransfer(customerAccountDto);
                }
            }

        }

        private CustomerExchangeTransferDto GenerateDto(List<Currency> allCurrencies, ExcelToTransactionDto item, string transactionType)
        {
            CustomerExchangeTransferDto customerAccountDto = new CustomerExchangeTransferDto();
            int customerId = Convert.ToInt32(item.Customer);
            string currencyCode = (transactionType == "Transfer") ? item.FromCurrency : item.Currency;
            Currency currency = allCurrencies.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
            CustomerAccountDto customerAccount = _customerBS.GetCustomerAccounts(customerId).Where(x => x.Currency == currency.Id).FirstOrDefault();

            if (item.TransactionType == "Giris")
            {
                customerAccountDto.CustomerId = customerId;
                customerAccountDto.FromAccountId = 0;
                customerAccountDto.ToAccountId = customerAccount.Id;
                customerAccountDto.TransactionDate = item.TransactionDate;
                customerAccountDto.TransactionType = ExchangeConversionType.Giris;
                customerAccountDto.CurrencyRate = 1;
                customerAccountDto.CurrencyBuyingRate = 1;
                customerAccountDto.CurrencySellingRate = 1;

            }
            else if (item.TransactionType == "Cikis")
            {
                customerAccountDto.CustomerId = customerId;
                customerAccountDto.FromAccountId = customerAccount.Id;
                customerAccountDto.ToAccountId = 0;
                customerAccountDto.TransactionDate = item.TransactionDate;
                customerAccountDto.TransactionType = ExchangeConversionType.Cikis;
                customerAccountDto.CurrencyRate = 1;
                customerAccountDto.CurrencyBuyingRate = 1;
                customerAccountDto.CurrencySellingRate = 1;
            }
            else if (item.TransactionType == "Transfer")
            {
                Currency fromCurrency = allCurrencies.Where(x => x.CurrencyCode == item.FromCurrency).FirstOrDefault();
                CustomerAccountDto fromAccount = _customerBS.GetCustomerAccounts(customerId).Where(x => x.Currency == fromCurrency.Id).FirstOrDefault();
                Currency toCurrency = allCurrencies.Where(x => x.CurrencyCode == item.ToCurrency).FirstOrDefault();
                CustomerAccountDto toAccount = _customerBS.GetCustomerAccounts(customerId).Where(x => x.Currency == toCurrency.Id).FirstOrDefault();

                customerAccountDto.CustomerId = customerId;
                customerAccountDto.FromAccountId = customerAccount.Id;
                customerAccountDto.ToAccountId = toAccount.Id;
                customerAccountDto.TransactionDate = item.TransactionDate;
                customerAccountDto.TransactionType = ExchangeConversionType.Transfer;
                customerAccountDto.CurrencyRate = 1;
                customerAccountDto.CurrencyBuyingRate = Convert.ToDecimal(item.BuyingRate);
                customerAccountDto.CurrencySellingRate = Convert.ToDecimal(item.SellingRate);
            }
            decimal amount = 0;
            if (item.Amount == "tamami")
            {
                amount = (decimal)customerAccount.AccountBalance;
            }
            else if (item.Amount == "yarisi")
            {
                amount = (decimal)customerAccount.CalculatedLastAccountBalance / 2;
            }
            else
            {
                amount = Convert.ToDecimal(item.Amount);
            }
            customerAccountDto.Amount = amount;

            return customerAccountDto;
        }

    }
}
