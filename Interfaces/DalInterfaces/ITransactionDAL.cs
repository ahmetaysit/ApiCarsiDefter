using Core;
using Library.Entities;
using Interfaces.Repository;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DALINterfaces
{
    public interface ITransactionDAL : IBaseDAL,IGenericRepository<Transaction>
    {
        Transaction Add(Transaction transaction);
        Transaction Add(Transaction transaction, bool isBulk);
        List<Transaction> GetAllUnProccessedByDate(int customerAccountId,DateTime beforeDate);
        CustomerAccount GetAccount(int accountId);
        bool ProcessAllTransactionsByDate(int customerAccountId, DateTime beforeDate, Transaction tran, bool isSaving);
        List<Transaction> GetByDate(int customerId, DateTime date, int transactionType);
        bool Delete(Transaction oldTransaction);
        bool DeleteAll(List<Transaction> oldTransactions);
        List<Transaction> GetAllForShopProfit(int customerAccountId, DateTime beforeDate);

        void UpdateShopProfitTrans(DateTime beforeDate, bool iShopProfitTaken, Transaction newTran);
        List<Transaction> GetAfterDate(int customerId, DateTime date);
        List<CustomerTransactionHistoryDto> GetDayProfitHistory(DateTime startDate, DateTime endDate);
        void DeleteAllTransactions();
        List<Transaction> GetBetweenDate(int customerAccountId, DateTime startDate, DateTime endDate);
        DateTime GetMaxTransactionDate();
    }
}
