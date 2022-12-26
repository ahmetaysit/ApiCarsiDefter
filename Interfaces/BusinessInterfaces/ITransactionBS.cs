using Core;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Repository;

namespace Interfaces.BusinessIntefaces
{
    public interface ITransactionBS : IBaseBS, IGenericRepository<Transaction>
    {
        bool Add(TransactionDto transactionDto);
        bool Add(TransactionDto transactionDto, Customer customer, int poolCustomerId, List<CustomerAccount> accounts, List<Transaction> oldTransactions);
        bool AddBulk(BulkTransactionDto bulkTransactionDto);
        Transaction Insert(Transaction transaction);
        bool TakeProfitComission(CustomerExchangeTransferDto customerExchangeTransferDto, 
            Customer customer, CustomerAccount custAcc, int poolCustomerId, 
            List<CustomerAccountDto> poolAccounts,List<Transaction> alltransactions, bool isAvoidSave,
            Dictionary<Transaction, List<Transaction>> dic = null);
        List<Transaction> GetByDate(int customerId, DateTime date, int transactionType);
        bool AddShopProfit(ShopProfitEntryDto dto, int poolCustomerId, List<CustomerAccountDtoEx> customerAccountDtos, List<Transaction> allTransactions, Dictionary<Transaction, DateTime> dic);
        List<Transaction> GetAfterDate(int customerId, DateTime date);
        List<CustomerTransactionHistoryDto> GetDayProfitHistory(DateTime startDate, DateTime endDate);
        void DeleteAllTransactions();
        void RewindTransactions(DateTime rewindDate);
        DateTime GetMaxTransactionDate();

    }
}
