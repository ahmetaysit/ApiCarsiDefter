using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class ShopProfitEntryDto
    {
        public decimal TotalAmount { get; set; }
        public int DayCount { get; set; }
        public decimal ProfitAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CurrencyId { get; set; }
        public List<int> CustomerIds { get; set; }

    }
}
