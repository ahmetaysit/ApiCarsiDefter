using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class GroupSummaryDto
    {
        public string GroupName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal PoolProfit { get; set; }
    }
}
