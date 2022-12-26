using Interfaces.DALINterfaces;
using Core;
using Library.Entities;
using Library.Dto;
using Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Repository;

namespace DAL
{
    public class CustomerAccountDAL : GenericRepository<CustomerAccount>, ICustomerAccountDAL
    {
        ProjectDbContext _db;
        public CustomerAccountDAL(ProjectDbContext db):base(db)
        {
            _db = db;
        }
        public bool ChangeBalance(int accountId, decimal amount)
        {
            bool result = false;
            CustomerAccountBalance accountBalance = _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == accountId && x.IsActive == true).FirstOrDefault();

            if (accountBalance != null)
            {
                accountBalance.AccountBalance = amount;
                result = _db.SaveChanges() > 0;
            }


            return result;
        }

        public CustomerAccountBalance GetBalance(int customerAccountId)
        {
           return _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == customerAccountId ).FirstOrDefault();
        }

        public bool DeleteAccount(int customerAccountId,bool isAvoidSave)
        {
            var result = true;
            _db.CustomerAccount.Where(x => x.Id == customerAccountId).First().IsActive = false;
            _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == customerAccountId).First().IsActive = false;
            if (!isAvoidSave) result = _db.SaveChanges()>0;
            return result;
        }

        public CustomerAccount CreateNewWithBalance(CustomerAccount customerAccount, decimal amount)
        {

            CustomerAccount account = _db.CustomerAccount.Add(customerAccount).Entity;

            _db.SaveChanges();

            CustomerAccountBalance balance = new CustomerAccountBalance { CustomerAccountId = account.Id, AccountBalance = amount, IsActive = true };
            _db.CustomerAccountBalance.Add(balance);
            _db.SaveChanges();
            customerAccount.Id = account.Id;
            return customerAccount;
        }

        public void ResetAllAccountBalances()
        {
            _db.CustomerAccountBalance.ToList().ForEach(x => x.AccountBalance = 0);
            _db.SaveChanges();
        }

        public void DeleteAllAccounts()
        {
            string sql = @"truncate table Customer
                        truncate table CustomerAccount
                        truncate table CustomerAccountBalance
                        update Settings set SettingValue = 1 where SettingKey = 'PoolCustomer'";
            _db.Database.ExecuteQuery(sql);

        }
    }
}
