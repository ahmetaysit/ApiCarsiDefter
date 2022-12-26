using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class AddShopProfitDto
    {
        public decimal TotalAmount { get; set; }
        public int DayCount { get; set; }
        public decimal TotalProfit { get; set; }
        public DateTime SelectedDate { get; set; }
        public List<Currency> SelectedCurrencies { get; set; }
        public List<Customer> SelectedCustomers { get; set; }
    }
}
