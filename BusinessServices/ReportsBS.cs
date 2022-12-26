using BusinessServices;
using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Interfaces.DALINterfaces;
using Library.Responses;
using Library.Requests;

namespace BusinessServices
{
    public class ReportsBS : IReportsBS
    {
        IReportsDAL _reportDal;
        public ReportsBS(IReportsDAL reportDal)
        {
            _reportDal = reportDal;
        }
        public List<GroupSummaryDto> GetGroupSummaryReport(DateTime startDate, DateTime endDate)
        {
            return _reportDal.GetGroupSummaryReport(startDate, endDate);
        }

        public List<ImmediateStateDailyProfitDto> ImmediateStateDailyProfit(DateTime startDate, DateTime endDate)
        {
            return _reportDal.ImmediateStateDailyProfit(startDate, endDate);
        }

        public List<SummaryDto> ImmediateStateGroupSummary(DateTime stateDate,DateTime endDate)
        {
            return _reportDal.ImmediateStateGroupSummary(stateDate, endDate);
        }

        public List<SummaryDto> ImmediateStateSummary(DateTime stateDate,DateTime endDate)
        {
            return _reportDal.ImmediateStateSummary(stateDate, endDate);
        }

        public List<CustomerTransactionHistoryDto> GetCustomerTransactionHistory(int customerId, DateTime startDate, DateTime endDate)
        {
            return _reportDal.GetCustomerTransactionHistory(customerId, startDate, endDate);
        }

        public List<CustomerTransactionHistoryDtoExtention> GetGeneralTransactionHistory(DateTime startDate, DateTime endDate)
        {
            return _reportDal.GetGeneralTransactionHistory(startDate, endDate);
        }

        public List<GetCustomerBackupResponse> GetCustomerBackup()
        {
            return _reportDal.GetCustomerBackup();
        }

        public List<ImmediateStateDailyProfitDto> GetMonthlyProfitHistory(DateTime startDate, DateTime endDate)
        {
            var result = _reportDal.GetMonthlyProfitHistory(startDate, endDate);

            int index = 0;
            decimal? previous = 1;
            foreach (var item in result)
            {                
                previous *= item.TLProfit;

                item.TLTotalProfit = previous;
            }

            previous = 1;
            foreach (var item in result)
            {
                previous *= item.USDProfit;

                item.USDTotalProfit = previous;
            }

            previous = 1;
            foreach (var item in result)
            {
                previous *= item.EURProfit;

                item.EURTotalProfit = previous;
            }

            previous = 1;
            foreach (var item in result)
            {
                previous *= item.GAUProfit;

                item.GAUTotalProfit = previous;
            }

            previous = 1;
            foreach (var item in result)
            {
                previous *= item.GBPProfit;

                item.GBPTotalProfit = previous;
            }

            return result;
        }
    
        public List<GetDayProfitHistoryWithCumulative> GetDayProfitHistoryWithCumulative(GetDashBoardReportsRequest request)
        {
            return _reportDal.GetDayProfitHistoryWithCumulative(request);
        }
        public object GetLast50Transaction()
        {
            return _reportDal.GetLast50Transaction();
        }
    }
}
