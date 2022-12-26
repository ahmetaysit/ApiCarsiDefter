using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Library.Entities;
using Library.Responses;
using Library.Requests;

namespace Interfaces.BusinessIntefaces
{
    public interface IReportsBS : IBaseBS
    {
        List<GroupSummaryDto> GetGroupSummaryReport(DateTime startDate, DateTime endDate);
        List<SummaryDto> ImmediateStateSummary(DateTime stateDate, DateTime endDate);
        List<SummaryDto> ImmediateStateGroupSummary(DateTime stateDate, DateTime endDate);
        List<ImmediateStateDailyProfitDto> ImmediateStateDailyProfit(DateTime startDate, DateTime endDate);
        List<CustomerTransactionHistoryDto> GetCustomerTransactionHistory(int customerId, DateTime startDate, DateTime endDate);
        List<CustomerTransactionHistoryDtoExtention> GetGeneralTransactionHistory(DateTime startDate, DateTime endDate);
        List<GetCustomerBackupResponse> GetCustomerBackup();
        List<ImmediateStateDailyProfitDto> GetMonthlyProfitHistory(DateTime startDate, DateTime endDate);
        List<GetDayProfitHistoryWithCumulative> GetDayProfitHistoryWithCumulative(GetDashBoardReportsRequest request);

        object GetLast50Transaction();
    }
}
