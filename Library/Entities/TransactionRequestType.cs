using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{
    [Table("TransactionRequestType")]
    public class TransactionRequestType
    {
        public int Id { get; set; }
        public int TypeCode { get; set; }
        public string TypeName { get; set; }
    }
}
