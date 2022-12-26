using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Requests
{
    public class GetCustomerTransactionHistoryRequest : BaseDateFilter
    {
        public Customer Customer { get; set; }
    }

    public class BaseDateFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
