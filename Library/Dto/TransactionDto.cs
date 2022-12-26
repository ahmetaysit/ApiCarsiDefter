using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class TransactionDto
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        //public int? CustomerAccountId { get; set; }

        public int? TransactionType { get; set; }

        public decimal? TransactionAmount { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? CurrencyRate { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal BuyingRate { get; set; }

        public decimal SellingRate { get; set; }

        public bool? IsProcessed { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
