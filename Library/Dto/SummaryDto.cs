using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dto
{
    public class SummaryDto
    {
        public string GroupName { get; set; }
        public decimal TL { get; set; }
        public decimal USD { get; set; }
        public decimal EUR { get; set; }
        public decimal GAU { get; set; }

        public decimal GBP { get; set; }
    }
}
