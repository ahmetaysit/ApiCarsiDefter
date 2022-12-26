using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using System.Data.SqlClient;
using System.Data;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Library.Responses;
using Library.Requests;
using Library.Enums;

namespace DAL
{
    public class ReportsDAL : IReportsDAL
    {
        ProjectDbContext _db;
        public ReportsDAL(ProjectDbContext db)
        {
            _db = db;
        }

        public List<GroupSummaryDto> GetGroupSummaryReport(DateTime startDate, DateTime endDate)
        {
            string cmd = @"
select * from (
	select gr.Id,gr.GroupName,
            cur.CurrencyCode,
            isnull((select sum(tra.TransactionAmount)
            from [Transaction] tra
            left join CustomerAccount ca on ca.Id = tra.CustomerAccountId 
            left join Customer c on c.Id = ca.CustomerId
            where tra.TransactionType in (1,3) and ca.Currency = cur.Id
            and c.IsJustForBalance = 0
            and c.CustomerGroupId = gr.Id
            and tra.TransactionDate between @startDate and @endDate
            ),0) - isnull((select sum(tra.TransactionAmount)
            from [Transaction] tra
            left join CustomerAccount ca on ca.Id = tra.CustomerAccountId 
            left join Customer c on c.Id = ca.CustomerId
            where tra.TransactionType = 11 and ca.Currency = cur.Id
            and c.CustomerGroupId = gr.Id
            and c.IsJustForBalance = 0
            and tra.TransactionDate between @startDate and @endDate
            ),0) TotalProfit,
            isnull((select SUM(cab.AccountBalance) from CustomerAccount ca
            left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id
            left join Customer c on c.Id = ca.CustomerId
            where ca.Currency = cur.Id
            and c.IsJustForBalance = 0
            and c.CustomerGroupId = gr.Id),0) TotalBalance
            ,isnull((select SUM(TransactionAmount) from PoolIncomeFromGroup 
            pig left join CustomerAccount gca on gca.Id = pig.PoolCustomerAccountId 
            where pig.FromGroupId = gr.Id and cur.Id = gca.Currency
            and pig.TransactionDate between @startDate and @endDate
            ),0) PoolProfit
            from CustomerGroup gr
            left join Currency cur on 1=1
            --order by GroupName,cur.Id
			union all
			select 0,'Hepsi' GroupName,
            cur.CurrencyCode,
            isnull((select sum(tra.TransactionAmount)
            from [Transaction] tra
            left join CustomerAccount ca on ca.Id = tra.CustomerAccountId 
            left join Customer c on c.Id = ca.CustomerId
            where tra.TransactionType in (1,3) and ca.Currency = cur.Id
            and c.IsJustForBalance = 0
            and tra.TransactionDate between @startDate and @endDate
            ),0) - isnull((select sum(tra.TransactionAmount)
            from [Transaction] tra
            left join CustomerAccount ca on ca.Id = tra.CustomerAccountId 
            left join Customer c on c.Id = ca.CustomerId
            where tra.TransactionType = 11 and ca.Currency = cur.Id
            and c.IsJustForBalance = 0
            and tra.TransactionDate between @startDate and @endDate
            ),0) TotalProfit,
            isnull((select SUM(cab.AccountBalance) from CustomerAccount ca
            left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id
            left join Customer c on c.Id = ca.CustomerId
            where ca.Currency = cur.Id
            and c.IsJustForBalance = 0
            ),0) TotalBalance
            ,isnull((select SUM(TransactionAmount) from PoolIncomeFromGroup 
            pig left join CustomerAccount gca on gca.Id = pig.PoolCustomerAccountId 
            where  cur.Id = gca.Currency
            and pig.TransactionDate between @startDate and @endDate
            ),0) PoolProfit
            from Currency cur 
			) A order by Id";

            Dictionary<string, object> prmList = new Dictionary<string, object>();
            prmList.Add("startDate", startDate);
            prmList.Add("endDate", endDate);
            var result = _db.Database.ExecuteQuery<GroupSummaryDto>(cmd, prmList);
            return result;
        }


        public List<SummaryDto> ImmediateStateSummary(DateTime stateDate, DateTime endDate)
        {
            string cmd = @"select * from (
                            select'Hepsi' GroupName, c.CurrencyCode,SUM(cab.AccountBalance) TotalBalance from Currency c
                            left join CustomerAccount ca on ca.Currency = c.Id and ca.IsActive = 1
                            left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id and cab.IsActive = 1
                            left join Customer cus on cus.Id = ca.CustomerId
                            where cus.IsJustForBalance = 0
                            group by c.CurrencyCode
                            ) A
                            pivot
                            (
                            SUM(TotalBalance)
                            for CurrencyCode in ([TL],[USD],[EUR],[GAU],[GBP])
                            ) A
                            ";
            List<SummaryDto> list = _db.Database.ExecuteQuery<SummaryDto>(cmd);
            return list;

        }

        public List<SummaryDto> ImmediateStateGroupSummary(DateTime stateDate, DateTime endDate)
        {
            string cmd = @"
            select * from (
            select cg.GroupName, c.CurrencyCode,SUM(isnull(cab.AccountBalance,0)) TotalBalance from CustomerGroup cg
            left join Customer cus on cus.CustomerGroupId = cg.Id and cus.IsActive = 1
            left join CustomerAccount ca on ca.CustomerId = cus.Id and ca.IsActive = 1
            left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id and cab.IsActive = 1
            left join Currency c on c.Id = ca.Currency
            where c.CurrencyCode is not null
            and cus.IsJustForBalance = 0
            group by c.CurrencyCode,cg.GroupName
            union all
            select cus.CustomerName, c.CurrencyCode,SUM(isnull(cab.AccountBalance,0)) TotalBalance from Customer cus
            left join CustomerAccount ca on ca.CustomerId = cus.Id and ca.IsActive = 1
            left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id and cab.IsActive = 1
            left join Currency c on c.Id = ca.Currency
            where cus.CustomerName = 'Ana Havuz Hesap'
            and cus.IsJustForBalance = 0
            group by c.CurrencyCode,cus.CustomerName
            ) A
            pivot
            (
            SUM(TotalBalance)
            for CurrencyCode in ([TL],[USD],[EUR],[GAU])
            ) A";
            List<SummaryDto> list = _db.Database.ExecuteQuery<SummaryDto>(cmd);
            return list;

        }

        public List<ImmediateStateDailyProfitDto> ImmediateStateDailyProfit(DateTime startDate, DateTime endDate)
        {
            string cmd = @"select 
            TransactionDate,
            MAX(TLBuying) TLBuying,
            MAX(TLSelling) TLSelling,
            MAX(TLSelling)/MAX(TLBuying) TLProfit,
            1.00 TLTotalProfit,
            MAX(USDBuying) USDBuying,
            MAX(USDSelling) USDSelling,
            MAX(USDSelling)/MAX(USDBuying) USDProfit,
            1.00 USDTotalProfit,
            MAX(EURBuying) EURBuying,
            MAX(EURSelling) EURSelling,
            MAX(EURSelling)/MAX(EURBuying) EURProfit,
            1.00 EURTotalProfit,
            MAX(GAUBuying) GAUBuying,
            MAX(GAUSelling) GAUSelling,
            MAX(GAUSelling)/MAX(GAUBuying) GAUProfit,
            1.00 GAUTotalProfit
            from (SELECT * FROM (select distinct TransactionDate,
            c.CurrencyCode + 'Buying' CurBuying,
            c.CurrencyCode + 'Selling' CurSelling,
            BuyingRate,
            SellingRate,
            (SellingRate/BuyingRate) -1 DailyProfit
            from Currency c
            left join [Transaction] t on t.CurrencyId = c.Id
            where t.TransactionType = 1
            and t.TransactionDate between @startDate and @endDate) A
            pivot
            (
            SUM(SellingRate)
            for CurSelling in ([TLSelling],[USDSelling],[EURSelling],[GAUSelling])
            ) A
            pivot
            (
            SUM(BuyingRate)
            for CurBuying in ([TLBuying],[USDBuying],[EURBuying],[GAUBuying])
            ) B
            ) A
            group by TransactionDate";
            Dictionary<string, object> parameterList  = new Dictionary<string, object>();
            parameterList.Add("startDate", startDate);
            parameterList.Add("endDate", endDate);
            List<ImmediateStateDailyProfitDto> list = _db.Database.ExecuteQuery<ImmediateStateDailyProfitDto>(cmd, parameterList);
            return list;
        }

        public List<CustomerTransactionHistoryDto> GetCustomerTransactionHistory(int customerId, DateTime startDate, DateTime endDate)
        {
            var result = (from tran in _db.Transaction
                          join customer in _db.Customer on tran.CustomerId equals customer.Id
                          from customerAccount in _db.CustomerAccount.Where(customerAccount => tran.CustomerAccountId == customerAccount.Id).DefaultIfEmpty()
                          from balance in _db.CustomerAccountBalance.Where(balance => customerAccount.Id == balance.CustomerAccountId).DefaultIfEmpty()
                          from currency in _db.Currency.Where(currency => customerAccount.Currency == currency.Id).DefaultIfEmpty()
                          from tranTypes in _db.TransactionTypes.Where(tranTypes => tran.TransactionType == tranTypes.TransactionCode).DefaultIfEmpty()
                          from customerGroup in _db.CustomerGroup.Where(customerGroup => customer.CustomerGroupId == customerGroup.Id).DefaultIfEmpty()
                          from fromCustomerAccount in _db.CustomerAccount.Where(fromCustomerAccount => fromCustomerAccount.Id == tran.FromAccountId).DefaultIfEmpty()
                          from fromCustomer in _db.Customer.Where(fromCustomer => fromCustomer.Id == fromCustomerAccount.CustomerId).DefaultIfEmpty()
                              //from percent in _db.Transaction.Where(percent => customerAccount.Id == percent.CustomerAccountId && percent.TransactionType == 1 )
                          where (customer.Id == customerId || customerId == 0) &&
                          tran.TransactionDate >= startDate &&
                          tran.TransactionDate <= endDate &&
                          //customerAccount.IsActive == true &&
                          customer.IsActive == true
                          //balance.IsActive == true
                          select new CustomerTransactionHistoryDto
                          {
                              TransactionId = tran.Id,
                              TransactionType = tranTypes.TransactionExplanation,
                              TransactionAmount = tran.TransactionAmount,
                              TransactionDate = tran.TransactionDate,
                              BuyingRate = tran.BuyingRate,
                              SellingRate = tran.SellingRate,
                              IsProcessed = tran.IsProcessed,
                              IsShopProfitTaken = tran.IsShopProfitTaken,
                              CustomerId = customer.Id,
                              CustomerName = customer.CustomerName,
                              CustomerCode = customer.CustomerCode,
                              PhoneNumber = customer.PhoneNumber.ToString(),
                              PoolRate = customer.PoolRate,
                              AccountNo = customerAccount.AccountNo,
                              CurrencyCode = currency.CurrencyCode,
                              AccountBalance = (decimal)balance.AccountBalance,
                              BalanceBeforeTransaction = (decimal)tran.BalanceBeforeTransaction,
                              GroupName = customerGroup.GroupName,
                              TransactionDescription = tran.Description,
                              FirstBalance = (decimal)(_db.Transaction.Where(percent => customerAccount.Id == percent.CustomerAccountId && percent.TransactionType == 1 && percent.TransactionDate >= startDate
                                         && percent.TransactionDate <= endDate).FirstOrDefault().BalanceBeforeTransaction),
                              LastBalance = (decimal)(_db.Transaction.Where(percent => customerAccount.Id == percent.CustomerAccountId && percent.TransactionType == 1 && percent.TransactionDate >= startDate
                              && percent.TransactionDate <= endDate).OrderByDescending(x => x.Id).Select(x => new { LastBalance = x.TransactionAmount + x.BalanceBeforeTransaction }).FirstOrDefault().LastBalance),
                              FromCustomerName = fromCustomer.CustomerName
                          }).OrderBy(x => x.TransactionId).ToList();
            return result;
        }

        public List<CustomerTransactionHistoryDtoExtention> GetGeneralTransactionHistory(DateTime startDate, DateTime endDate)
        {
            string cmd = @"select tra.Id,
                    TransactionType = tt.TransactionExplanation,
                    TransactionAmount = tra.TransactionAmount,
                    TransactionDate = tra.TransactionDate,
                    CreationDate = tra.CreationDate,
                    BuyingRate = tra.BuyingRate,
                    SellingRate = tra.SellingRate,
                    IsProcessed = tra.IsProcessed,
                    IsShopProfitTaken = tra.IsShopProfitTaken,
                    CustomerId = c.Id,
                    CustomerName = c.CustomerName,
                    CustomerCode = c.CustomerCode,
                    PhoneNumber = c.PhoneNumber,
                    PoolRate = c.PoolRate,
                    AccountNo = ca.AccountNo,
                    CurrencyCode = cur.CurrencyCode,
                    AccountBalance = cab.AccountBalance,
                    BalanceBeforeTransaction = tra.BalanceBeforeTransaction,
                    GroupName = cg.GroupName 
                    from [Transaction] tra
                    left join Customer  c on c.Id = tra.CustomerId
                    left join CustomerAccount ca on ca.Id = tra.CustomerAccountId
                    left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id
                    left join Currency cur on cur.Id = ca.Currency
                    left join TransactionTypes tt on tt.TransactionCode = tra.TransactionType
                    left join CustomerGroup cg on cg.Id = c.CustomerGroupId
                    where tra.TransactionDate >= @startDate and
                    tra.TransactionDate <= @endDate and
                    tra.TransactionType != 1 and
                    c.IsActive = 1
                    union all
                    select distinct 0,
                    TransactionType = tt.TransactionExplanation,
                    TransactionAmount = 0,
                    TransactionDate = tra.TransactionDate,
                    CreationDate = tra.CreationDate,
                    BuyingRate = tra.BuyingRate,
                    SellingRate = tra.SellingRate,
                    IsProcessed = 0,
                    IsShopProfitTaken = 0,
                    CustomerId = 0,
                    CustomerName = '',
                    CustomerCode = '',
                    PhoneNumber = '',
                    PoolRate = 0,
                    AccountNo = '',
                    CurrencyCode = cur.CurrencyCode,
                    AccountBalance = 0,
                    BalanceBeforeTransaction = 0,
                    GroupName = ''
                    from [Transaction] tra
                    left join TransactionTypes tt on tt.TransactionCode = tra.TransactionType
					left join Currency cur on cur.Id = tra.CurrencyId
                    where TransactionType = 1 and TransactionDate between @startDate and @endDate 
                    --order by TransactionDate desc";

            Dictionary<string, object> prmList = new Dictionary<string, object>();
            prmList.Add("startDate", startDate);
            prmList.Add("endDate", endDate);
            var result = _db.Database.ExecuteQuery<CustomerTransactionHistoryDtoExtention>(cmd, prmList);
            
            return result;
        }

        public List<GetCustomerBackupResponse> GetCustomerBackup()
        {
            string cmd = @"select pv.*
                        ,pv.TL-(pv.TLProfit * pv.PoolRate /100) TLLastBalance
                        ,pv.USD-(pv.USDProfit * pv.PoolRate /100) USDLastBalance
                        ,pv.EUR-(pv.EURProfit * pv.PoolRate /100) EURLastBalance
                        ,pv.GAU-(pv.GAUProfit * pv.PoolRate /100) GAULastBalance
                        ,pv.GBP-(pv.GBPProfit * pv.PoolRate /100) GBPLastBalance
                        from 
                        (select *
                        ,isnull((select SUM(t.TransactionAmount) from [Transaction] t left join CustomerAccount ca on ca.Id = t.CustomerAccountId 
                        where t.CustomerId = pivott.Id and ca.Currency = 1 and t.IsProcessed = 0 and t.TransactionType in (1,3)),0) TLProfit
                        ,isnull((select SUM(t.TransactionAmount) from [Transaction] t left join CustomerAccount ca on ca.Id = t.CustomerAccountId 
                        where t.CustomerId = pivott.Id and ca.Currency = 2 and t.IsProcessed = 0 and t.TransactionType in (1,3)),0) USDProfit
                        ,isnull((select SUM(t.TransactionAmount) from [Transaction] t left join CustomerAccount ca on ca.Id = t.CustomerAccountId 
                        where t.CustomerId = pivott.Id and ca.Currency = 3 and t.IsProcessed = 0 and t.TransactionType in (1,3)),0) EURProfit
                        ,isnull((select SUM(t.TransactionAmount) from [Transaction] t left join CustomerAccount ca on ca.Id = t.CustomerAccountId 
                        where t.CustomerId = pivott.Id and ca.Currency = 4 and t.IsProcessed = 0 and t.TransactionType in (1,3)),0) GAUProfit
                        ,isnull((select SUM(t.TransactionAmount) from [Transaction] t left join CustomerAccount ca on ca.Id = t.CustomerAccountId 
                        where t.CustomerId = pivott.Id and ca.Currency = 5 and t.IsProcessed = 0 and t.TransactionType in (1,3)),0) GBPProfit
                        from (
                        select c.*,CONVERT(date, getdate()) TransactionDate,cab.AccountBalance,cur.CurrencyCode
                        from Customer c
                        left join CustomerAccount ca on ca.CustomerId = c.Id
                        left join CustomerAccountBalance cab on cab.CustomerAccountId = ca.Id
                        left join Currency cur on cur.Id = ca.Currency
                        ) As pv
                        PIVOT
                        (
                        SUM(AccountBalance)
                        for CurrencyCode in ([TL],[USD],[EUR],[GAU],[GBP])
                        ) as pivott
                        ) pv";

            var result = _db.Database.ExecuteQuery<GetCustomerBackupResponse>(cmd);
            return result;
        }

        public List<ImmediateStateDailyProfitDto> GetMonthlyProfitHistory(DateTime startDate, DateTime endDate)
        {
            string sql = @"select distinct tra.TransactionDate
                    ,isnull(tl.BuyingRate,1) TLBuying,isnull(tl.SellingRate,1) TLSelling , isnull(tl.SellingRate,1)/isnull(tl.BuyingRate,1) TLProfit 
                    ,isnull(usd.BuyingRate,1) USDBuying,isnull(usd.SellingRate,1) USDSelling, isnull(usd.SellingRate,1)/isnull(usd.BuyingRate,1) USDProfit 
                    ,isnull(eur.BuyingRate,1) EURBuying,isnull(eur.SellingRate,1) EURSelling, isnull(EUR.SellingRate,1)/isnull(EUR.BuyingRate,1) EURProfit
                    ,isnull(gau.BuyingRate,1) GAUBuying,isnull(gau.SellingRate,1) GAUSelling, isnull(GAU.SellingRate,1)/isnull(GAU.BuyingRate,1) GAUProfit
                    ,isnull(gbp.BuyingRate,1) GBPBuying,isnull(gbp.SellingRate,1) GBPSelling, isnull(GBP.SellingRate,1)/isnull(GBP.BuyingRate,1) GBPProfit
                    from [Transaction] tra
                    left join (select distinct TransactionDate traDate,traTl.BuyingRate,traTl.SellingRate from [Transaction] traTl where traTl.TransactionType = 1 and traTl.CurrencyId = 1) tl on tl.traDate = tra.TransactionDate
                    left join (select distinct TransactionDate traDate,traTl.BuyingRate,traTl.SellingRate from [Transaction] traTl where traTl.TransactionType = 1 and traTl.CurrencyId = 2) usd on usd.traDate = tra.TransactionDate
                    left join (select distinct TransactionDate traDate,traTl.BuyingRate,traTl.SellingRate from [Transaction] traTl where traTl.TransactionType = 1 and traTl.CurrencyId = 3) eur on eur.traDate = tra.TransactionDate
                    left join (select distinct TransactionDate traDate,traTl.BuyingRate,traTl.SellingRate from [Transaction] traTl where traTl.TransactionType = 1 and traTl.CurrencyId = 4) gau on gau.traDate = tra.TransactionDate
                    left join (select distinct TransactionDate traDate,traTl.BuyingRate,traTl.SellingRate from [Transaction] traTl where traTl.TransactionType = 1 and traTl.CurrencyId = 5) gbp on gbp.traDate = tra.TransactionDate
                    where tra.TransactionType = 1
                    and tra.TransactionDate between @startDate and @endDate
                    order by TransactionDate";

            Dictionary<string, object> prmList = new Dictionary<string, object>();
            prmList.Add("startDate", startDate);
            prmList.Add("endDate", endDate);
            var result = _db.Database.ExecuteQuery<ImmediateStateDailyProfitDto>(sql, prmList);

            return result;
        }


        public List<GetDayProfitHistoryWithCumulative> GetDayProfitHistoryWithCumulative(GetDashBoardReportsRequest request)
        {
            string cmd = @"select *,EXP(SUM(LOG((COALESCE(ProfitRate, 1)))) 
       OVER (partition by CurrencyCode ORDER BY TransactionDate)) CumulativeDailyProfitRate from (
select distinct 
TransactionDate,c.CurrencyCode,BuyingRate,SellingRate,SellingRate / BuyingRate as ProfitRate
from [Transaction] t
left join Currency c on c.Id = t.CurrencyId
where TransactionType = 1
and TransactionDate between @startDate and @endDate
) A ";
            Dictionary<string, object> parameterList = new Dictionary<string, object>();
            parameterList.Add("startDate", request.StartDate.Date);
            parameterList.Add("endDate", request.EndDate.Date);
            List<GetDayProfitHistoryWithCumulative> list = _db.Database.ExecuteQuery<GetDayProfitHistoryWithCumulative>(cmd, parameterList);
            return list;
        }

        public object GetLast50Transaction()
        {
            List<int> getTypes = new List<int> {
            (int)TransactionType.ManuelIncome,
            (int)TransactionType.ManuelOutCome
            }; 
            var transactions = _db.Transaction.Where(x=> getTypes.Contains(x.TransactionType.Value)).OrderByDescending(x=> x.Id).Take(50).ToList();
            var allCustomers = _db.Customer.ToList();
            var allCurrencies = _db.Currency.ToList();
            var transactionMinDate = transactions.Min(x => x.TransactionDate);
            var customers = _db.Customer.Where(x => x.IsActive == true && !x.IsJustForBalance && x.CreatedDate >= transactionMinDate).OrderByDescending(x => x.CreatedDate).Take(50).ToList();
            var tmpRes = new List<LastFiftyTransactionDto>();
            transactions.ForEach(x =>
            {
                var currency = allCurrencies.FirstOrDefault(y => y.Id == x.CurrencyId).CurrencyCode;
                var customerName = allCustomers.FirstOrDefault(y => y.Id == x.CustomerId).CustomerName;
                var transactionName = x.TransactionType < 10 ? "Giriş" : "Çıkış";
                var description = $"{transactionName} : {customerName} müşterisine {x.TransactionDate.ToString("dd.MM.yyyy")} tarihinde {x.TransactionAmount} {currency}";
                tmpRes.Add(new LastFiftyTransactionDto
                {
                    Date = x.TransactionDate,
                    Description = description
                });
            });

            customers.ForEach(x =>
            {
                tmpRes.Add(new LastFiftyTransactionDto
                {
                    Date = x.CreationDate != null ? x.CreationDate.Value : x.CreatedDate.Value,
                    Description = $"{x.CustomerName} müşterisi açıldı."
                });
            });

            var res = tmpRes.OrderByDescending(x => x.Date).ToList().Take(50).ToList();
            
            return res;
        }
    }
}
