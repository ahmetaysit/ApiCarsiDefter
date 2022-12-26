using Library.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Responses
{
    public class GetDashBoardReportsResponse
    {
        public List<SummaryDto> ImmediateStateSummary { get; set; }
        public List<SummaryDto> ImmediateStateGroupSummary { get; set; }
        public List<ImmediateStateDailyProfitDto> ImmediateStateDailyProfit { get; set; }
    }
}
