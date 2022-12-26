using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class LastFiftyTransactionDto
    {
        public DateTime Date { get; set; }

        public string DateStr
        {
            get { return Date.ToString("dd.MM.yyyy"); }
        }

        public string Description { get; set; }
    }
}
