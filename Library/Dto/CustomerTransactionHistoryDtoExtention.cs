using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Dto
{
    public class CustomerTransactionHistoryDtoExtention : CustomerTransactionHistoryDto
    {
        public DateTime CreationDate { get; set; }
    }
}
