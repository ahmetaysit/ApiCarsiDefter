using Core;
using Library.Entities;
using Interfaces.BusinessIntefaces;
using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Library.Enums;
using BusinessServices.Repository;

namespace BusinessServices
{
    public class PoolIncomeFromGroupBS : GenericRepositoryBs<IPoolIncomeFromGroupDAL, PoolIncomeFromGroup>, IPoolIncomeFromGroupBS
    {
        IPoolIncomeFromGroupDAL _poolIncomeFromGroupDal;
        ICustomerGroupDAL _customerGroupDal;
        public PoolIncomeFromGroupBS(IPoolIncomeFromGroupDAL dal,
            ICustomerGroupDAL customerGroupDal) : base(dal)
        {
            _poolIncomeFromGroupDal = dal;
            _customerGroupDal = customerGroupDal;
        }

        public void AddDailyProfit(Transaction tran)
        {
            List<PoolIncomeFromGroup> lst = _poolIncomeFromGroupDal.GetByFilter(X => X.PoolCustomerAccountId == tran.CustomerAccountId).ToList();

            var groups = _customerGroupDal.GetAll();
            foreach (var group in groups)
            {
                decimal totalAmount = lst.Where(x => x.FromGroupId == group.Id && x.TransactionType < 10).Sum(x => x.TransactionAmount);

                if (totalAmount > 0)
                {
                    decimal? amount = (totalAmount * tran.SellingRate / tran.BuyingRate) - totalAmount;
                    PoolIncomeFromGroup poolIncome = new PoolIncomeFromGroup
                    {
                        FromGroupId = group.Id,
                        PoolCustomerAccountId = (int)tran.CustomerAccountId,
                        PoolCustomerId = (int)tran.CustomerId,
                        TransactionAmount = (decimal)amount,
                        TransactionDate = (DateTime)tran.TransactionDate,
                        TransactionType = (int)tran.TransactionType,
                        IsShopProfitTaken = false
                    };

                    _poolIncomeFromGroupDal.Insert(poolIncome);
                }

            }


        }

        public void AddShopProfit(ShopProfitEntryDto dto, CustomerAccountDto customerAccount, int customerId)
        {
            //throw new NotImplementedException();
            var groups = _customerGroupDal.GetAll();
            foreach (var group in groups)
            {
                List<PoolIncomeFromGroup> transactions = _poolIncomeFromGroupDal.GetByFilter(x => x.FromGroupId == group.Id && x.PoolCustomerAccountId == customerAccount.Id && x.TransactionDate <= dto.TransactionDate && x.IsShopProfitTaken == false).ToList();

                List<DateTime> manuelOutDates = transactions.Where(x => x.TransactionType == (int)TransactionType.ManuelIncome
                    && transactions.Where(y => y.Id < x.Id && y.TransactionType == 1 && x.IsShopProfitTaken == false).Count() > 0
                    ).Select(x => x.TransactionDate).ToList();

                if (!manuelOutDates.Contains(dto.TransactionDate))
                    manuelOutDates.Add(dto.TransactionDate);

                //decimal? firstBalance = transactions.Where(x => x.TransactionDate <= manuelOutDates[0] && x.TransactionType == 1 && x.IsShopProfitTaken == false)
                //    .Select(x => x.TransactionAmount).FirstOrDefault();

                //firstBalance = firstBalance == null ? 0 : firstBalance;
                decimal? firstBalance = transactions.Where(x => x.TransactionType == (int)TransactionType.ManuelIncome).Select(x => x.TransactionAmount).FirstOrDefault();
                DateTime tmpDate = dto.TransactionDate;
                if (transactions.Count > 0)
                {
                    foreach (DateTime item in manuelOutDates)
                    {
                        if (manuelOutDates[0] != item)
                        {
                            DateTime lastBalanceDate = transactions.Where(x => x.TransactionDate < item && x.IsShopProfitTaken == false && x.TransactionType == (int)TransactionType.ManuelIncome).Select(x => x.TransactionDate).LastOrDefault();
                            firstBalance = transactions.Where(x => x.TransactionDate <= lastBalanceDate)
        .Sum(x => x.TransactionAmount);

                        }

                        if (!firstBalance.HasValue)
                        {
                            firstBalance = 0;
                        }

                        var totalProfit = transactions.Where(x => x.TransactionDate <= item && x.TransactionType == 1).Sum(x => x.TransactionAmount);
                        var totalOutcome = transactions.Where(x => x.TransactionDate <= item && x.TransactionType > 10).Sum(x => x.TransactionAmount);
                        var dayCount = transactions.Where(x => x.TransactionDate <= item && x.TransactionType == 1).Count();
                        List<PoolIncomeFromGroup> tmpTransactions = transactions.Where(x => x.TransactionDate <= item && x.TransactionType < 10).ToList();

                        if (tmpDate != dto.TransactionDate)
                        {
                            totalProfit = transactions.Where(x => x.TransactionDate <= item && x.TransactionDate > tmpDate && x.TransactionType < 10).Sum(x => x.TransactionAmount);
                            totalOutcome = transactions.Where(x => x.TransactionDate <= item && x.TransactionDate > tmpDate && x.TransactionType > 10).Sum(x => x.TransactionAmount);
                            dayCount = transactions.Where(x => x.TransactionDate <= item && x.TransactionDate > tmpDate && x.TransactionType < 10).Count();
                            tmpTransactions = transactions.Where(x => x.TransactionDate <= item && x.TransactionDate > tmpDate && x.TransactionType < 10).ToList();
                        }

                        var profit = firstBalance * dayCount * dto.ProfitAmount / (dto.TotalAmount * dto.DayCount);

                        PoolIncomeFromGroup groupTran = new PoolIncomeFromGroup
                        {
                            PoolCustomerId = customerId,
                            PoolCustomerAccountId = customerAccount.Id,
                            TransactionType = (int)TransactionType.ShopProfitIncome,
                            TransactionAmount = (decimal)profit,
                            TransactionDate = dto.TransactionDate,
                            IsShopProfitTaken = true,
                            FromGroupId = group.Id
                        };

                        _poolIncomeFromGroupDal.Insert(groupTran);



                        tmpDate = item;
                    }
                }
            }
        }
    }
}
