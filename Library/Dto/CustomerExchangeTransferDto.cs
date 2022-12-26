using Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class CustomerExchangeTransferDto
    {
        public ExchangeConversionType TransactionType { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal CurrencyBuyingRate { get; set; }
        public decimal CurrencySellingRate { get; set; }
        public bool IsEndOfMonth { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public int Currency { get; set; }
    }
}
