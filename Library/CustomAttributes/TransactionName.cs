using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.CustomAttributes
{
    public class TransactionName : Attribute
    {
        public string Name { get; set; }
    }
}
