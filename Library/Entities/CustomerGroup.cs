using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities
{
    [Table("CustomerGroup")]
    public class CustomerGroup
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }

    }
}
