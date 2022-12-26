using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Enums;
using Library.Dto;
using DataLayer.Repository;
using DataLayer;
using Library.Helpers;

namespace DAL
{
    public class TransactionDAL : GenericRepository<Transaction>, ITransactionDAL
    {
        ProjectDbContext _db;
        public TransactionDAL(ProjectDbContext db)
            : base(db)
        {
            _db = db;
        }
        public Transaction Add(Transaction transaction)
        {
            bool result = true;
            Transaction newValue = null;
            if (transaction.TransactionAmount != 0)
            {
                var balance = _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == transaction.CustomerAccountId).FirstOrDefault();

                transaction.BalanceBeforeTransaction = balance.AccountBalance;
                newValue = _db.Transaction.Add(transaction).Entity;


                if (transaction.TransactionType < 10) balance.AccountBalance += transaction.TransactionAmount;
                if (transaction.TransactionType > 10) balance.AccountBalance -= transaction.TransactionAmount;

                result = _db.SaveChanges() > 0;
            }

            return newValue;
        }
        public Transaction Add(Transaction transaction, bool isBulk)
        {
            bool result = true;
            Transaction newValue = null;
            if (transaction.TransactionAmount != 0)
            {
                var balance = _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == transaction.CustomerAccountId).FirstOrDefault();

                transaction.BalanceBeforeTransaction = balance.AccountBalance;
                _db.Transaction.Add(transaction);


                if (transaction.TransactionType < 10) balance.AccountBalance += transaction.TransactionAmount;
                if (transaction.TransactionType > 10) balance.AccountBalance -= transaction.TransactionAmount;
                if (!isBulk)
                    result = _db.SaveChanges() > 0;
            }

            return transaction;
        }

        public bool Delete(Transaction oldTransaction)
        {
            bool result = true;
            _db.Transaction.Remove(oldTransaction);

            var balance = _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == oldTransaction.CustomerAccountId).FirstOrDefault();
            if (oldTransaction.TransactionType < 10) balance.AccountBalance -= oldTransaction.TransactionAmount;
            if (oldTransaction.TransactionType > 10) balance.AccountBalance += oldTransaction.TransactionAmount;

            result = _db.SaveChanges() > 0;

            return result;
        }

        public bool DeleteAll(List<Transaction> oldTransactions)
        {
            bool result = true;
            if (oldTransactions.Count > 0)
            {
                var balance = _db.CustomerAccountBalance.Where(x => x.CustomerAccountId == oldTransactions[0].CustomerAccountId).FirstOrDefault();
                foreach (var oldTransaction in oldTransactions)
                {
                    _db.Transaction.Remove(oldTransaction);
                    if (oldTransaction.TransactionType < 10) balance.AccountBalance -= oldTransaction.TransactionAmount;
                    if (oldTransaction.TransactionType > 10) balance.AccountBalance += oldTransaction.TransactionAmount;
                }
                result = _db.SaveChanges() > 0;
            }
            return result;
        }
        public List<Transaction> GetByDate(int customerId, DateTime date, int transactionType)
        {
            var result = _db.Transaction.Where(x => x.TransactionDate.Date == date.Date);

            if (customerId > 0)
                result = result.Where(x => x.CustomerAccountId == customerId);
            if (transactionType > 0)
                result = result.Where(x => x.TransactionType == transactionType);

            return result.ToList();
        }

        public List<Transaction> GetAfterDate(int customerId, DateTime date)
        {
            var result = _db.Transaction.Where(x => x.TransactionDate > date);
            if (customerId > 0)
                result = result.Where(x => x.CustomerId == customerId);
            return result.ToList();
        }

        public bool ProcessAllTransactionsByDate(int customerAccountId, DateTime beforeDate, Transaction tran, bool isAvoidSaving = false)
        {
            var deneme = _db.Transaction.Where(x => x.TransactionDate <= beforeDate && x.CustomerAccountId == customerAccountId && x.IsProcessed == false).ToList();
            _db.Transaction.Where(x => x.TransactionDate <= beforeDate && x.CustomerAccountId == customerAccountId && x.IsProcessed == false).ToList().ForEach(c =>
            {
                c.IsProcessed = true;
                c.ProcessTransactionId = tran.Id;
            });
            var result = true;
            if (!isAvoidSaving)
                result = _db.SaveChanges() > 0;
            return result;
        }

        public List<Transaction> GetAllUnProccessedByDate(int customerAccountId, DateTime beforeDate)
        {

            return _db.Transaction.Where(x => x.TransactionDate <= beforeDate && x.CustomerAccountId == customerAccountId && x.IsProcessed == false).ToList();
        }

        public List<Transaction> GetAllForShopProfit(int customerAccountId, DateTime beforeDate)
        {

            return _db.Transaction.Where(x => x.TransactionDate <= beforeDate && x.CustomerAccountId == customerAccountId && x.IsShopProfitTaken == false).ToList();
        }

        public CustomerAccount GetAccount(int accountId)
        {

            return _db.CustomerAccount.Where(x => x.Id == accountId).First();
        }

        public List<Transaction> GetBetweenDate(int customerAccountId, DateTime startDate, DateTime endDate)
        {
            return _db.Transaction.Where(x => x.CustomerAccountId == customerAccountId && x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();
        }

        public List<CustomerTransactionHistoryDto> GetDayProfitHistory(DateTime startDate, DateTime endDate)
        {
            string sql = @"select distinct TransactionDate,BuyingRate,SellingRate,cur.CurrencyCode from [Transaction] tra
left join Currency cur on cur.Id = tra.CurrencyId
where TransactionType = 1 and TransactionDate between @startDate and @endDate order by TransactionDate desc";

            Dictionary<string, object> prmList = new Dictionary<string, object>();
            prmList.Add("startDate", startDate);
            prmList.Add("endDate", endDate);
            var result = _db.Database.ExecuteQuery<CustomerTransactionHistoryDto>(sql, prmList);
            
            return result;
        }

        public void UpdateShopProfitTrans(DateTime beforeDate, bool iShopProfitTaken, Transaction newTran)
        {

            var updateValue = _db.Transaction.Where(x => x.TransactionDate <= beforeDate && x.IsShopProfitTaken == false && x.CustomerAccountId == newTran.CustomerAccountId).ToList();

            foreach (var item in updateValue)
            {
                item.IsShopProfitTaken = true;
                item.ShopProfitTransactionId = newTran.Id;
            }

            _db.SaveChanges();
        }

        public void DeleteAllTransactions()
        {
            //TODO:Düzelt
            _db.Database.ExecuteQuery("TRUNCATE TABLE [Transaction]");
        }

        public DateTime GetMaxTransactionDate()
        {
            string cmd = @"select isnull(MAX(TransactionDate),cast(-53690 as datetime)) from [Transaction] where TransactionType = 1";
            var result = _db.Database.ExecuteQuery<DateTime>(cmd);

            return result[0];
        }
    }
}
