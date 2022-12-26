using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities
{
    public class PoolIncomeFromGroup
    {
        public int Id { get; set; }

        public int FromGroupId { get; set; }

        public int PoolCustomerId { get; set; }

        public int PoolCustomerAccountId { get; set; }

        public decimal TransactionAmount { get; set; }
        
        public DateTime TransactionDate { get; set; }

        public int TransactionType { get; set; }

        public bool IsShopProfitTaken { get; set; }
    }
}
