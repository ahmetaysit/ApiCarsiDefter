using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities
{
    [Table("TransactionTypes")]
    public partial class TransactionTypes
    {
        public int Id { get; set; }

        public int TransactionCode { get; set; }

        [StringLength(50)]
        public string TransactionName { get; set; }

        [StringLength(50)]
        public string TransactionExplanation { get; set; }

    }
}
