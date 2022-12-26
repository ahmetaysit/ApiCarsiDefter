namespace Library.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CustomerAccountBalance")]
    public partial class CustomerAccountBalance
    {
        public int Id { get; set; }

        public int? CustomerAccountId { get; set; }

        public decimal? AccountBalance { get; set; }

        public bool? IsActive { get; set; }
    }
}
