using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    [Table("TransactionRequest")]
    public class TransactionRequest
    {
        public int Id { get; set; }
        public int TransactionType { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal ToAccBalanceAfter { get; set; }
        public decimal FromAccBalanceAfter { get; set; }
        public decimal BuyingRate { get; set; }
        public decimal SellingRate { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Description { get; set; }
    }
}
