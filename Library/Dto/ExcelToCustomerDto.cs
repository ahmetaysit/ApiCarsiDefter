using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class ExcelToCustomerDto
    {
        public string CustomerName { get; set; }
        public long? PhoneNumber { get; set; }
        public string Email { get; set; }
        public int PoolRate { get; set; }
        public int? CustomerGroupId { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<ExcelCustomerAccountDto> Accounts { get; set; }

    }

    public class ExcelCustomerAccountDto
    {
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
