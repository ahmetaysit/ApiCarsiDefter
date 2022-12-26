using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Responses
{
    public class GetCustomerBackupResponse
    {
        public long Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public long? DefaultCurrencyId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public long PoolRate { get; set; }
        public bool IsActive { get; set; }
        public long? CustomerGroupId { get; set; }
        public bool IsJustForBalance { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public double Tl { get; set; }
        public double Usd { get; set; }
        public double Eur { get; set; }
        public double Gau { get; set; }
        public double Gbp { get; set; }
        public double TlProfit { get; set; }
        public double UsdProfit { get; set; }
        public double EurProfit { get; set; }
        public double GauProfit { get; set; }
        public double GbpProfit { get; set; }
        public double TlLastBalance { get; set; }
        public double UsdLastBalance { get; set; }
        public double EurLastBalance { get; set; }
        public double GauLastBalance { get; set; }
        public double GbpLastBalance { get; set; }
    }
}
