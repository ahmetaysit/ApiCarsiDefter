namespace Library.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Transaction")]
    public partial class Transaction
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public int? CustomerAccountId { get; set; }

        public int? TransactionType { get; set; }

        public decimal? TransactionAmount { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? CurrencyRate { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal? BuyingRate { get; set; }

        public decimal? SellingRate { get; set; }

        public bool? IsProcessed { get; set; }

        public bool? IsShopProfitTaken { get; set; }

        public DateTime CreationDate { get; set; }

        public int? ShopProfitTransactionId { get; set; }

        public decimal? BalanceBeforeTransaction { get; set; }

        public int? FromAccountId { get; set; }

        public int? ProcessTransactionId { get; set; }
        public string Description { get; set; }
    }
}
