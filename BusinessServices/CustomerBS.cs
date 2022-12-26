using Core;
using Library.Entities;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.DALINterfaces;
using Library.Dto;
using Interfaces.BusinessIntefaces;

namespace BusinessServices
{
    public class CustomerBS : ICustomerBS
    {
        ICustomerDAL _customerDal;
        ICustomerAccountDAL _customerAccountDal;
        ICurrencyDAL _currencyDal;
        ICustomerAccountBS _customerAccountBS;
        ICustomerRequestBS _customerRequestBS;
        public CustomerBS(ICustomerDAL customerDal, ICustomerAccountDAL customerAccountDal, ICurrencyDAL currencyDal, ICustomerAccountBS customerAccountBS,ICustomerRequestBS customerRequestBS)
        {
            _customerDal = customerDal;
            _customerAccountDal = customerAccountDal;
            _currencyDal = currencyDal;
            _customerAccountBS = customerAccountBS;
            _customerRequestBS = customerRequestBS;
        }

        public Customer Get(int customerId)
        {
            return _customerDal.Get(customerId);
        }

        public List<CustomerAccountDto> GetCustomerAccounts(int customerId)
        {
            return _customerDal.GetCustomerAccounts(customerId);
        }

        public List<Customer> GetCustomers()
        {
            return _customerDal.GetCustomers();
        }

        public Customer AddCustomer(Customer customer)
        {
            return _customerDal.SaveCustomer(customer);
        }

        //TODO:Buraya hesaplarıda otomatik eklemeli yapılacak
        public bool SaveCustomer(Customer customer)
        {
            int customerId = customer.Id;
            Customer newCustomer = _customerDal.SaveCustomer(customer);
            if (customerId < 1)
            {
                List<Currency> currencies = _currencyDal.GetAllCurrencies();

                foreach (Currency item in currencies)
                {
                    CustomerAccount customerAccount = new CustomerAccount { AccountNo = item.CurrencyCode + " Hesap 001", Currency = item.Id, CustomerId = customer.Id, IsActive = true };
                    _customerAccountDal.CreateNewWithBalance(customerAccount, 0);
                }
            }

            return true;
        }

        public List<CustomerGroup> GetCustomerGroups()
        {
            return _customerDal.GetCustomerGroups();
        }

        public List<Customer> GetCustomersForJustBalance()
        {
            return _customerDal.GetCustomersForJustBalance();
        }

        public List<CustomerAccountDtoEx> GetAllCustomerAccounts()
        {
            return _customerDal.GetAllCustomerAccounts();
        }

        public bool ApproveCustomerRequest(int id)
        {
            var customer = _customerRequestBS.GetByFilter(x => x.Id == id).FirstOrDefault();
            customer.Status = 2;
            _customerRequestBS.Update(customer);
            Customer cst = new Customer
            {
                Id = 0,
                CustomerCode = "",
                CustomerGroupId = customer.CustomerGroupId,
                CustomerName = customer.CustomerName,
                DefaultCurrencyId = customer.Currency,
                Email = "",
                IsActive = true,
                IsJustForBalance = false,
                PhoneNumber = 0,
                PoolRate = customer.PoolRate,
                CreationDate = customer.TransactionDate.Date
            };

            Customer newCustomer = _customerDal.SaveCustomer(cst);
            List<Currency> currencies = _currencyDal.GetAllCurrencies();

            foreach (Currency item in currencies)
            {
                decimal amount = 0;
                if (item.Id == customer.Currency) amount = customer.Amount;
                CustomerAccount customerAccount = new CustomerAccount { AccountNo = item.CurrencyCode + " Hesap 001", Currency = item.Id, CustomerId = newCustomer.Id, IsActive = true };
                _customerAccountDal.CreateNewWithBalance(customerAccount, amount);

                //if (item.Id == customer.Currency)
                //{
                //    amount = customer.Amount;
                //    CustomerExchangeTransferDto exchangeDto = new CustomerExchangeTransferDto
                //    {
                //        Amount = amount,
                //        CurrencyBuyingRate = 1,
                //        CurrencyRate = 1,
                //        CurrencySellingRate = 1,
                //        CustomerId = cst.Id,
                //        TransactionDate = customer.TransactionDate,
                //        TransactionType = Library.Enums.ExchangeConversionType.Giris,
                //        IsEndOfMonth = false,
                //        ToAccountId = customerAccount.Id
                //    };
                //    _customerAccountBS.ExchangeTransfer(exchangeDto);
                //}
            }

            return true;
        }
        public bool RejectCustomerRequest(int id)
        {
            var customer = _customerRequestBS.GetByFilter(x => x.Id == id).FirstOrDefault();
            customer.Status = 3;
            _customerRequestBS.Update(customer);
            return true;
        }
        public bool CreateCustomerWithBalance(AddCustomerSummaryDto customer)
        {
            CustomerRequest entity = new CustomerRequest
            {
                Amount = customer.Amount,
                Currency = customer.Currency,
                CustomerGroupId = customer.CustomerGroupId,
                CustomerName = customer.CustomerName,
                Id = 0,
                PoolRate = 20,
                Status = 1,
                TransactionDate = customer.TransactionDate
            };
            _customerRequestBS.Insert(entity);
            //Customer cst = new Customer
            //{
            //    Id = customer.Id,
            //    CustomerCode = customer.CustomerCode,
            //    CustomerGroupId = customer.CustomerGroupId,
            //    CustomerName = customer.CustomerName,
            //    DefaultCurrencyId = customer.DefaultCurrencyId,
            //    Email = customer.Email,
            //    IsActive = true,
            //    IsJustForBalance = false,
            //    PhoneNumber = customer.PhoneNumber,
            //    PoolRate = customer.PoolRate,
            //    CreationDate = customer.TransactionDate.Date
            //};

            //Customer newCustomer = _customerDal.SaveCustomer(cst);
            //List<Currency> currencies = _currencyDal.GetAllCurrencies();

            //foreach (Currency item in currencies)
            //{
            //    decimal amount = 0;
            //    if (item.Id == customer.Currency) amount = customer.Amount;
            //    CustomerAccount customerAccount = new CustomerAccount { AccountNo = item.CurrencyCode + " Hesap 001", Currency = item.Id, CustomerId = newCustomer.Id, IsActive = true };
            //    _customerAccountDal.CreateNewWithBalance(customerAccount, amount);

            //    //if (item.Id == customer.Currency)
            //    //{
            //    //    amount = customer.Amount;
            //    //    CustomerExchangeTransferDto exchangeDto = new CustomerExchangeTransferDto
            //    //    {
            //    //        Amount = amount,
            //    //        CurrencyBuyingRate = 1,
            //    //        CurrencyRate = 1,
            //    //        CurrencySellingRate = 1,
            //    //        CustomerId = cst.Id,
            //    //        TransactionDate = customer.TransactionDate,
            //    //        TransactionType = Library.Enums.ExchangeConversionType.Giris,
            //    //        IsEndOfMonth = false,
            //    //        ToAccountId = customerAccount.Id
            //    //    };
            //    //    _customerAccountBS.ExchangeTransfer(exchangeDto);
            //    //}
            //}

            return true;
        }
    }
}
