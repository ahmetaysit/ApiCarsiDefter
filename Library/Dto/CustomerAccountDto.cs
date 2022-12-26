using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class CustomerAccountDto
    {
        public int Id { get; set; }
        public string AccountNo { get; set; }
        public string CurrencyName { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal CalculatedLastAccountBalance { get; set; }
        public decimal AmountToPool { get; set; }
        public int Currency { get; set; }
        public bool IsActive { get; set; }
    }
    public class CustomerAccountDtoEx : CustomerAccountDto
    {
        public int CustomerId { get; set; }
    }
}
