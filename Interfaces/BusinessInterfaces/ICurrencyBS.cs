using Core;
using Library.Dto;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.BusinessIntefaces
{
    public interface ICurrencyBS : IBaseBS
    {
        List<Currency> GetAllCurrencies();
    }
}
