using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class BulkTransactionDto
    {
        public decimal BuyingRate { get; set; }
        public decimal SellingRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<Currency> SelectedCurrencies { get; set; }
        public List<Customer> SelectedCustomer { get; set; }
    }
}
