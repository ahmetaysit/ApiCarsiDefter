using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    [Table("TransactionRequestStatus")]
    public class TransactionRequestStatus
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public string StatusName { get; set; }
    }
}
