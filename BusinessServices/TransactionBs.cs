using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Interfaces.DALINterfaces;
using Core;
using Library.Entities;
using Library.Enums;
using System.Diagnostics;
using BusinessServices.Repository;

namespace BusinessServices
{
    public class TransactionBs : GenericRepositoryBs<ITransactionDAL, Transaction>, ITransactionBS
    {
        ITransactionDAL _transactionDal;
        ICustomerDAL _customerDal;
        ISettingsDAL _settingsDal;
        ICustomerAccountDAL _customerAccountDal;
        IPoolIncomeFromGroupDAL _poolIncomeFromGroupDal;
        IPoolIncomeFromGroupBS _poolIncomeFromGroupBs;
        public TransactionBs(
            ITransactionDAL transactionDal,
            ICustomerDAL customerDal,
            ISettingsDAL settingsDal,
            ICustomerAccountDAL customerAccountDal,
            IPoolIncomeFromGroupDAL poolIncomeFromGroupDal,
            IPoolIncomeFromGroupBS poolIncomeFromGroupBs
            ) : base(transactionDal)
        {
            _transactionDal = transactionDal;
            _customerDal = customerDal;
            _settingsDal = settingsDal;
            _customerAccountDal = customerAccountDal;
            _poolIncomeFromGroupDal = poolIncomeFromGroupDal;
            _poolIncomeFromGroupBs = poolIncomeFromGroupBs;
        }

        public Transaction Insert(Transaction transaction)
        {
            return _transactionDal.Add(transaction);
        }

        public bool Add(TransactionDto transactionDto)
        {
            //herkes için ekle
            List<Customer> customers = transactionDto.CustomerId > 0 ? _customerDal.GetCustomers().Where(x => x.Id == transactionDto.CustomerId).ToList() : _customerDal.GetCustomers();
            Settings setting = _settingsDal.Get("PoolCustomer");

            int poolCustomerId = Convert.ToInt32(_settingsDal.Get("PoolCustomer").Settingvalue);

            foreach (Customer customer in customers)
            {
                List<CustomerAccountDto> accounts = _customerDal.GetCustomerAccounts(customer.Id).Where(x => x.Currency == transactionDto.CurrencyId && x.IsActive).ToList();
                foreach (var account in accounts)
                {
                    if (account != null)
                    {
                        List<Transaction> oldTransactions = _transactionDal.GetByDate(account.Id, (DateTime)transactionDto.TransactionDate, 1);
                        foreach (Transaction oldtran in oldTransactions)
                        {
                            _transactionDal.Delete(oldtran);
                        }

                        decimal? newBalance = _customerDal.GetCustomerAccounts(customer.Id).Where(x => x.Id == account.Id).FirstOrDefault().AccountBalance;

                        decimal? amount = (newBalance * transactionDto.SellingRate / transactionDto.BuyingRate) - newBalance;

                        Transaction tran = new Transaction
                        {
                            CustomerId = customer.Id,
                            CustomerAccountId = account.Id,
                            TransactionType = (int)TransactionType.Income,
                            TransactionAmount = amount,
                            CurrencyId = transactionDto.CurrencyId,
                            TransactionDate = transactionDto.TransactionDate,
                            BuyingRate = transactionDto.BuyingRate,
                            SellingRate = transactionDto.SellingRate,
                            IsProcessed = false,
                            IsShopProfitTaken = false,
                            CreationDate = DateTime.Now
                        };

                        _transactionDal.Add(tran);

                        if (transactionDto.CustomerId == poolCustomerId && amount > 0)
                        {
                            _poolIncomeFromGroupBs.AddDailyProfit(tran);
                        }
                    }
                }


            }

            return true;
        }

        public bool Add(TransactionDto transactionDto, Customer customer, int poolCustomerId, List<CustomerAccount> accounts, List<Transaction> oldTransactions)
        {
            foreach (var account in accounts.Where(x => x.Currency == transactionDto.CurrencyId && x.IsActive == true).ToList())
            {
                if (account != null)
                {
                    List<Transaction> oldTransactionsForCustomer = oldTransactions.Where(x => x.CustomerAccountId == account.Id).ToList();
                    foreach (Transaction oldtran in oldTransactionsForCustomer)
                    {
                        _transactionDal.Delete(oldtran);
                    }

                    decimal? newBalance = _customerAccountDal.GetBalance(account.Id).AccountBalance;

                    decimal? amount = (newBalance * transactionDto.SellingRate / transactionDto.BuyingRate) - newBalance;

                    Transaction tran = new Transaction
                    {
                        CustomerId = customer.Id,
                        CustomerAccountId = account.Id,
                        TransactionType = (int)TransactionType.Income,
                        TransactionAmount = amount,
                        CurrencyId = transactionDto.CurrencyId,
                        TransactionDate = transactionDto.TransactionDate,
                        BuyingRate = transactionDto.BuyingRate,
                        SellingRate = transactionDto.SellingRate,
                        IsProcessed = false,
                        IsShopProfitTaken = false,
                        CreationDate = transactionDto.CreationDate,
                    };

                    _transactionDal.Add(tran, true);

                    if (transactionDto.CustomerId == poolCustomerId && amount > 0)
                    {
                        _poolIncomeFromGroupBs.AddDailyProfit(tran);
                    }
                }

            }

            return true;
        }

        public bool AddBulk(BulkTransactionDto bulkTransactionDto)
        {
            Settings setting = _settingsDal.Get("PoolCustomer");

            int poolCustomerId = Convert.ToInt32(setting.Settingvalue);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var allAccounts = _customerAccountDal.GetAll().ToList();
            var oldTransactions = _transactionDal.GetByDate(0, (DateTime)bulkTransactionDto.TransactionDate, 1);
            DateTime creationDate = DateTime.Now;
            foreach (var customer in bulkTransactionDto.SelectedCustomer.Where(x=> x.CreationDate == null || x.CreationDate.Value.Date <= bulkTransactionDto.TransactionDate.Date))
            {
                var customerAccounts = allAccounts.Where(x => x.IsActive == true && x.CustomerId == customer.Id).ToList();
                foreach (var currency in bulkTransactionDto.SelectedCurrencies)
                {
                    TransactionDto transactionDto = new TransactionDto();
                    transactionDto.BuyingRate = bulkTransactionDto.BuyingRate;
                    transactionDto.SellingRate = bulkTransactionDto.SellingRate;
                    transactionDto.TransactionDate = bulkTransactionDto.TransactionDate.Date;
                    transactionDto.CurrencyId = currency.Id;
                    transactionDto.CustomerId = customer.Id;
                    transactionDto.CreationDate = creationDate;
                    Add(transactionDto, customer, poolCustomerId, customerAccounts, oldTransactions);
                }
            }
            stopwatch.Stop();
            var elapset = stopwatch.Elapsed;
            _transactionDal.Save();

            return true;
        }
        public bool TakeProfitComission(CustomerExchangeTransferDto customerExchangeTransferDto, Customer customer,
            CustomerAccount custAcc, int poolCustomerId, List<CustomerAccountDto> poolCustomers, List<Transaction> alltransactions, bool isAvoidSave,
            Dictionary<Transaction, List<Transaction>> dic)
        {
            //Customer customer = _customerDal.Get(customerExchangeTransferDto.CustomerId);
            decimal poolRate = customer.PoolRate;
            var transactions = alltransactions.Where(x => x.CustomerAccountId == customerExchangeTransferDto.FromAccountId).ToList();
            //CustomerAccount custAcc = _transactionDal.GetAccount(customerExchangeTransferDto.FromAccountId);
            decimal? totalProfit = transactions.Sum(x => x.TransactionAmount);

            decimal? poolProfit = totalProfit * poolRate / 100;

            if (totalProfit != 0)
            {

                Transaction tran = new Transaction
                {
                    CustomerId = customerExchangeTransferDto.CustomerId,
                    CustomerAccountId = customerExchangeTransferDto.FromAccountId,
                    TransactionType = (int)TransactionType.OutCome,
                    TransactionAmount = poolProfit,
                    CurrencyId = custAcc.Currency,
                    TransactionDate = customerExchangeTransferDto.TransactionDate,
                    IsProcessed = true,
                    CreationDate = DateTime.Now,
                    IsShopProfitTaken = customerExchangeTransferDto.IsEndOfMonth
                };
                if (isAvoidSave) dic.Add(tran, transactions);

                var poolCustomerAccount = poolCustomers.Where(x => x.Currency == custAcc.Currency && x.IsActive == true).FirstOrDefault();

                Transaction tranPool = new Transaction
                {
                    CustomerId = poolCustomerId,
                    CustomerAccountId = poolCustomerAccount.Id,
                    TransactionType = (int)TransactionType.ManuelIncome,
                    TransactionAmount = poolProfit,
                    CurrencyId = custAcc.Currency,
                    TransactionDate = customerExchangeTransferDto.TransactionDate,
                    IsProcessed = true,
                    IsShopProfitTaken = customerExchangeTransferDto.IsEndOfMonth,
                    FromAccountId = customerExchangeTransferDto.FromAccountId,
                    CreationDate = DateTime.Now
                };

                _transactionDal.Add(tran, isAvoidSave);

                _transactionDal.Add(tranPool, isAvoidSave);
                if (tranPool.TransactionAmount > 0)
                {
                    PoolIncomeFromGroup poolIncomeFromGroup = new PoolIncomeFromGroup
                    {
                        FromGroupId = (int)customer.CustomerGroupId,
                        PoolCustomerAccountId = custAcc.Id,
                        PoolCustomerId = poolCustomerId,
                        TransactionAmount = (decimal)tranPool.TransactionAmount,
                        TransactionDate = (DateTime)tranPool.TransactionDate,
                        TransactionType = (int)tranPool.TransactionType,
                        IsShopProfitTaken = false
                    };

                    _poolIncomeFromGroupDal.Add(poolIncomeFromGroup);
                }

                _transactionDal.ProcessAllTransactionsByDate(customerExchangeTransferDto.FromAccountId, customerExchangeTransferDto.TransactionDate, tran, isAvoidSave);
            }


            return true;
        }

        public List<Transaction> GetByDate(int customerId, DateTime date, int transactionType)
        {
            return _transactionDal.GetByDate(customerId, date, transactionType);
        }

        public bool AddShopProfit(ShopProfitEntryDto dto, int poolCustomerId, List<CustomerAccountDtoEx> customerAccountDtos, List<Transaction> allTransactions, Dictionary<Transaction, DateTime> dic)
        {

            foreach (int customerId in dto.CustomerIds)
            {
                List<CustomerAccountDto> customerAccounts = customerAccountDtos.Where(x => x.CustomerId == customerId && x.Currency == dto.CurrencyId).ToList<CustomerAccountDto>();

                foreach (CustomerAccountDto customerAccount in customerAccounts)
                {
                    List<Transaction> transactions = allTransactions.Where(x => x.CustomerAccountId == customerAccount.Id).ToList();

                    List<DateTime> manuelOutDates = transactions.Where(x => x.TransactionType == ((customerId == poolCustomerId) ? (int)TransactionType.ManuelIncome : (int)TransactionType.ManuelOutCome)
                        && transactions.Where(y => y.Id < x.Id && y.TransactionType == 1 && x.IsShopProfitTaken == false).Count() > 0
                        ).Select(x => x.TransactionDate).ToList();
                    
                    //yeni ekledik
                    List<int> manuelOutIds = transactions.Where(x => x.TransactionType == ((customerId == poolCustomerId) ? (int)TransactionType.ManuelIncome : (int)TransactionType.ManuelOutCome)
                        && transactions.Where(y => y.Id < x.Id && y.TransactionType == 1 && x.IsShopProfitTaken == false).Count() > 0
                        ).Select(x => x.Id).ToList();

                    if (!manuelOutDates.Contains(dto.TransactionDate))
                    {
                        manuelOutDates.Add(dto.TransactionDate);
                        //yeni ekledik
                        if (transactions.Count > 0) 
                            manuelOutIds.Add(transactions.Max(x => x.Id));
                        else
                            manuelOutIds.Add(allTransactions.Max(x => x.Id));
                    }
                        //manuelOutDates.Add(dto.TransactionDate);

                    decimal? firstBalance = transactions.Where(x => x.TransactionDate <= manuelOutDates[0] && x.TransactionType == 1 && x.IsShopProfitTaken == false)
                        .Select(x => x.BalanceBeforeTransaction).FirstOrDefault();

                    firstBalance = firstBalance == null ? 0 : firstBalance;

                    DateTime tmpDate = dto.TransactionDate;
                    int index = 0;
                    foreach (DateTime item in manuelOutDates)
                    {
                        if (manuelOutDates[0] != item)
                        {
                            firstBalance = transactions.Where(x => x.TransactionDate <= item && x.Id > manuelOutIds[index - 1] && x.TransactionDate >= tmpDate && x.TransactionType == 1 && x.IsShopProfitTaken == false)
    .Select(x => x.BalanceBeforeTransaction).FirstOrDefault();

                        }

                        if (!firstBalance.HasValue)
                        {
                            firstBalance = 0;
                        }

                        var totalProfit = transactions.Where(x => x.TransactionDate <= item && x.Id <= manuelOutIds[index] && x.TransactionType == 1).Sum(x => x.TransactionAmount);
                        var totalOutcome = transactions.Where(x => x.TransactionDate <= item && x.Id <= manuelOutIds[index] && x.TransactionType > 10).Sum(x => x.TransactionAmount);
                        var dayCount = transactions.Where(x => x.TransactionDate <= item && x.Id <= manuelOutIds[index] && x.TransactionType == 1).Count();
                        List<Transaction> tmpTransactions = transactions.Where(x => x.TransactionDate <= item && x.Id < manuelOutIds[index] && x.TransactionType < 10).ToList();

                        if (tmpDate != dto.TransactionDate)
                        {
                            totalProfit = transactions.Where(x => x.TransactionDate <= item && x.Id >= manuelOutIds[index - 1] && x.Id <= manuelOutIds[index] && x.TransactionDate >= tmpDate && x.TransactionType < 10).Sum(x => x.TransactionAmount);
                            totalOutcome = transactions.Where(x => x.TransactionDate <= item && x.Id >= manuelOutIds[index - 1] && x.Id <= manuelOutIds[index] && x.TransactionDate >= tmpDate && x.TransactionType > 10).Sum(x => x.TransactionAmount);
                            dayCount = transactions.Where(x => x.TransactionDate <= item && x.Id >= manuelOutIds[index-1] && x.Id <= manuelOutIds[index] && x.TransactionDate >= tmpDate && x.TransactionType == 1).Count();
                            tmpTransactions = transactions.Where(x => x.TransactionDate <= item && x.Id >= manuelOutIds[index - 1] && x.Id <= manuelOutIds[index] && x.TransactionDate >= tmpDate && x.TransactionType < 10).ToList();
                        }

                        var profit = firstBalance * dayCount * dto.ProfitAmount / (dto.TotalAmount * dto.DayCount);

                        //firstBalance = firstBalance - totalOutcome + totalProfit;

                        Transaction tran = new Transaction
                        {
                            CustomerId = customerId,
                            CustomerAccountId = customerAccount.Id,
                            TransactionType = (int)TransactionType.ShopProfitIncome,
                            TransactionAmount = profit,
                            CurrencyId = dto.CurrencyId,
                            TransactionDate = dto.TransactionDate,
                            BuyingRate = 1,
                            SellingRate = 1,
                            IsProcessed = false,
                            IsShopProfitTaken = true,
                            CreationDate = DateTime.Now
                        };

                        _transactionDal.Add(tran,true);
                        dic.Add(tran, item);
                        //if (newtran != null)
                        //    _transactionDal.UpdateShopProfitTrans(item, true, newtran);

                        tmpDate = item;
                        index++;
                    }
                    if (customerId == poolCustomerId)
                        _poolIncomeFromGroupBs.AddShopProfit(dto, customerAccount, customerId);
                }
            }

            return true;
        }

        public List<CustomerTransactionHistoryDto> GetDayProfitHistory(DateTime startDate, DateTime endDate)
        {
            return _transactionDal.GetDayProfitHistory(startDate, endDate);
        }

        public List<Transaction> GetAfterDate(int customerId, DateTime date)
        {
            return _transactionDal.GetAfterDate(customerId, date);
        }

        public void DeleteAllTransactions()
        {
            _transactionDal.DeleteAllTransactions();
        }

        public void RewindTransactions(DateTime rewindDate)
        {
            List<Customer> customers = _customerDal.GetCustomers();

            foreach (Customer customer in customers)
            {
                List<CustomerAccountDto> accounts = _customerDal.GetCustomerAccounts(customer.Id);

                foreach (CustomerAccountDto account in accounts)
                {
                    var transactions = _transactionDal.GetBetweenDate(account.Id, rewindDate, DateTime.Now.AddDays(30)).OrderByDescending(x => x.Id).ToList();

                    decimal balance = 0;

                    if (transactions.Count > 0)
                    {
                        balance = (decimal)transactions.LastOrDefault().BalanceBeforeTransaction;

                        //TODO:TransactionId 11 varsa ProcessTransactionId 11 olanlar IsProcessed = false ve ProcessTransactionId null yap
                        var processTransactions = transactions.Where(x => x.TransactionType == (int)TransactionType.OutCome).ToList();

                        foreach (var processTransaction in processTransactions)
                        {
                            var trans = _transactionDal.GetByFilter(x => x.ProcessTransactionId == processTransaction.Id);
                            foreach (var item in trans)
                            {
                                item.IsProcessed = false;
                                item.ProcessTransactionId = null;
                                _transactionDal.Update(item);
                            }
                        }

                        //TODO:TransactionId 3 varsa ShopProfitTransactionId 3 olanlar IsShopProfitTaken = false ve ShopProfitTransactionId null yap
                        var shopProfitTransactions = transactions.Where(x => x.TransactionType == (int)TransactionType.ShopProfitIncome).ToList();

                        foreach (var shopProfitTransaction in shopProfitTransactions)
                        {
                            var trans = _transactionDal.GetByFilter(x => x.ShopProfitTransactionId == shopProfitTransaction.Id);
                            foreach (var item in trans)
                            {
                                item.IsShopProfitTaken = false;
                                item.ShopProfitTransactionId = null;
                                _transactionDal.Update(item);
                            }
                        }

                        //Transactions sil
                        foreach (var item in transactions)
                        {
                            _transactionDal.Delete(item);
                        }

                        //_transactionDal.Save();
                        //balance güncelle
                        _customerAccountDal.ChangeBalance(account.Id, balance);
                    }
                }
            }

        }

        public DateTime GetMaxTransactionDate()
        {
            return _transactionDal.GetMaxTransactionDate();
        }
    }
}
