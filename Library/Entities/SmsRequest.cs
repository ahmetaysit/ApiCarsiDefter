using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities
{

    [Table("SmsRequest")]
    public partial class SmsRequest
    {
        public int Id { get; set; }
        
        public int RequestId { get; set; }

        public int CustomerId { get; set; }

        public long PhoneNumber { get; set; }

        [StringLength(1000)]
        public string SmsText { get; set; }

        [StringLength(50)]
        public string ResponseCode { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
