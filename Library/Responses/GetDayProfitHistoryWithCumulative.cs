using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Responses
{
    public class GetDayProfitHistoryWithCumulative
    {
        public DateTime TransactionDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal BuyingRate { get; set; }
        public decimal SellingRate { get; set; }
        public decimal ProfitRate { get; set; }
        public decimal CumulativeDailyProfitRate { get; set; }
    }
}
