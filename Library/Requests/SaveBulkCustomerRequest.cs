using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Requests
{
    public class SaveBulkCustomerRequest
    {
        public string CustomerName { get; set; }
        public long? PhoneNumber { get; set; }
        public string Email { get; set; }
        public int PoolRate { get; set; }
        public int CustomerGroupId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TL { get; set; }
        public decimal USD { get; set; }
        public decimal EUR { get; set; }
        public decimal GAU { get; set; }
        public decimal GBP { get; set; }
        public decimal CHF { get; set; }
    }
}
