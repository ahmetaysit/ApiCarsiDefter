namespace Library.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CustomerAccount")]
    public partial class CustomerAccount
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        [StringLength(50)]
        public string AccountNo { get; set; }

        public int? Currency { get; set; }

        public bool? IsActive { get; set; }
    }
}
