using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class EnOfMonthTransactionDto
    {
        public DateTime SelectedDate { get; set; }
        public List<Customer> SelectedCustomer { get; set; }
    }
}
