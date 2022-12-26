using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class CustomerTransactionHistoryDto
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; }
        public decimal? TransactionAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? BuyingRate { get; set; }
        public decimal? SellingRate { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsShopProfitTaken { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
        public int PoolRate { get; set; }
        public string AccountNo { get; set; }
        public string CurrencyCode { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal BalanceBeforeTransaction { get; set; }
        public string GroupName { get; set; }
        public decimal FirstBalance { get; set; }
        public decimal LastBalance { get; set; }
        public string FromCustomerName { get; set; }
        public string TransactionDescription { get; set; }
    }
}
