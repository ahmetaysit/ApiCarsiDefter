using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class ImmediateStateDailyProfitDto
    {
        public DateTime TransactionDate { get; set; }
        public decimal? TLBuying { get; set; }
        public decimal? TLSelling { get; set; }
        public decimal? TLProfit { get; set; }
        public decimal? TLTotalProfit { get; set; }
        public decimal? USDBuying { get; set; }
        public decimal? USDSelling { get; set; }
        public decimal? USDProfit { get; set; }
        public decimal? USDTotalProfit { get; set; }
        public decimal? EURBuying { get; set; }
        public decimal? EURSelling { get; set; }
        public decimal? EURProfit { get; set; }
        public decimal? EURTotalProfit { get; set; }
        public decimal? GAUBuying { get; set; }
        public decimal? GAUSelling { get; set; }
        public decimal? GAUProfit { get; set; }
        public decimal? GAUTotalProfit { get; set; }

        public decimal? GBPBuying { get; set; }
        public decimal? GBPSelling { get; set; }
        public decimal? GBPProfit { get; set; }
        public decimal? GBPTotalProfit { get; set; }
    }
}
