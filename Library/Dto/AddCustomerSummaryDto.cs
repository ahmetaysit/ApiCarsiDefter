using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class AddCustomerSummaryDto
    {
        public int Id { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public int? DefaultCurrencyId { get; set; }

        public long? PhoneNumber { get; set; }

        public string Email { get; set; }

        public int PoolRate { get; set; }

        public bool? IsActive { get; set; }

        public int? CustomerGroupId { get; set; }
        public bool IsJustForBalance { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
