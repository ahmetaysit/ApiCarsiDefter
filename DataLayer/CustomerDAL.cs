using Core;
using Interfaces.DALINterfaces;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Library.Helpers;

namespace DAL
{
    public class CustomerDAL : ICustomerDAL
    {
        ProjectDbContext _db;
        public CustomerDAL(ProjectDbContext db2)
        {
            _db = db2;
        }
        public List<Customer> GetCustomers()
        {
            return _db.Customer.Where(x => x.IsJustForBalance == false).ToList();
        }

        public List<Customer> GetCustomersForJustBalance()
        {
            return _db.Customer.Where(x => x.IsJustForBalance == true).ToList();
        }

        public bool Save(Customer customer)
        {

            _db.Customer.Add(customer);
            return _db.SaveChanges() > 0;
        }

        public Customer Get(int customerId)
        {

            return _db.Customer.Where(x => x.Id == customerId && x.IsActive == true).FirstOrDefault();

        }

        public List<CustomerAccountDto> GetCustomerAccounts(int customerId)
        {
            var result = (from account in _db.CustomerAccount
                          join cust in _db.Customer on account.CustomerId equals cust.Id
                          join currency in _db.Currency on account.Currency equals currency.Id
                          join balance in _db.CustomerAccountBalance on account.Id equals balance.CustomerAccountId
                          where account.CustomerId == customerId &&
                          account.IsActive == true &&
                          account.IsActive == true &&
                          balance.IsActive == true
                          select new CustomerAccountDto
                          {
                              Id = account.Id,
                              AccountNo = account.AccountNo,
                              CurrencyName = currency.CurrencyName,
                              AccountBalance = balance.AccountBalance,
                              Currency = (int)account.Currency,
                              IsActive = (bool)account.IsActive,
                              AmountToPool = Math.Round(((from tran in _db.Transaction
                                                        where tran.CustomerAccountId == account.Id
                                                        && tran.TransactionType == 1
                                                        && tran.IsProcessed == false
                                                        select tran.TransactionAmount
                                                                                       ).Sum(x => x.HasValue ? x.Value : 0) * cust.PoolRate / 100),4),
                          }).ToList();
            result.ForEach(x => x.CalculatedLastAccountBalance = (decimal)x.AccountBalance - x.AmountToPool);
            return result;

        }
        public List<CustomerAccountDtoEx> GetAllCustomerAccounts()
        {
            var result = (from account in _db.CustomerAccount
                          join cust in _db.Customer on account.CustomerId equals cust.Id
                          join currency in _db.Currency on account.Currency equals currency.Id
                          join balance in _db.CustomerAccountBalance on account.Id equals balance.CustomerAccountId
                          where 
                          account.IsActive == true &&
                          account.IsActive == true &&
                          balance.IsActive == true
                          select new CustomerAccountDtoEx
                          {
                              Id = account.Id,
                              CustomerId = cust.Id,
                              AccountNo = account.AccountNo,
                              CurrencyName = currency.CurrencyName,
                              AccountBalance = balance.AccountBalance,
                              Currency = (int)account.Currency,
                              IsActive = (bool)account.IsActive,
                          }).ToList();
            result.ForEach(x => x.CalculatedLastAccountBalance = (decimal)x.AccountBalance - x.AmountToPool);
            return result;
        }

        public Customer SaveCustomer(Customer customer)
        {
            bool result = true;

            Customer newCustomer = customer;
            if (customer.Id < 1)
            {
                newCustomer = _db.Customer.Add(customer).Entity;
            }
            else
            {
                var entity = _db.Customer.Where(x => x.Id == customer.Id).FirstOrDefault();
                entity.CustomerCode = customer.CustomerCode;
                entity.CustomerName = customer.CustomerName;
                entity.DefaultCurrencyId = customer.DefaultCurrencyId;
                entity.Email = customer.Email;
                entity.IsActive = customer.IsActive;
                entity.PhoneNumber = customer.PhoneNumber;
                entity.PoolRate = customer.PoolRate;
            }

            _db.SaveChanges();
            newCustomer.Id = customer.Id;
            return newCustomer;
        }

        public List<CustomerGroup> GetCustomerGroups()
        {
            return _db.CustomerGroup.Where(x => x.IsActive == true).ToList();
        }
    }
}
