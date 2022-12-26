using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Responses
{
    public class TransactionRequestListItem
    {
        public int TransactionRequestId { get; set; }
        public string TranSactionType { get; set;}
        public int TranSactionTypeCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set;}
        public decimal Amount { get; set;}
        public decimal ToAccBalanceAfter { get; set; }
        public decimal FromAccBalanceAfter { get; set; }
        public string ToAccount { get; set;}
        public int ToAccountId { get; set; }
        public string FromAccount { get; set;}
        public int FromAccountId { get; set; }
        public decimal BuyingRate { get; set;}
        public decimal SellingRate { get; set;}
        public string Status { get; set;}
        public string CreatedUser { get; set;}
        public DateTime TransactionDate { get; set;}
        public DateTime CreatedDate { get; set;}
        public string Description { get; set; }
    }
}
