namespace Library.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customer")]
    public partial class Customer
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public int? DefaultCurrencyId { get; set; }

        public long? PhoneNumber { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public int PoolRate { get; set; }

        public bool? IsActive { get; set; }

        public int? CustomerGroupId { get; set; }
        public bool IsJustForBalance { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
