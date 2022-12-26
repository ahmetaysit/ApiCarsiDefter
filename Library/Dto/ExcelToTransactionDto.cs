using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class ExcelToTransactionDto
    {
        public string Customer { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string BuyingRate { get; set; }
        public string SellingRate { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
    }
}
