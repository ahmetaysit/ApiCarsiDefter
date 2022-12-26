using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities
{
    [Table("CustomerRequest")]
    public class CustomerRequest
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }

        public int PoolRate { get; set; }
        public int? CustomerGroupId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public int Status { get; set; }
    }
}
