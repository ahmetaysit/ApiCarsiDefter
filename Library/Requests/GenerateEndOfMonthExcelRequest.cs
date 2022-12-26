using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Requests
{
    public class GenerateEndOfMonthExcelRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
