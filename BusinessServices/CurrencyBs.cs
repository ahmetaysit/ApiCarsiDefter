using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Interfaces.DALINterfaces;

namespace BusinessServices
{
    public class CurrencyBs : ICurrencyBS
    {
        ICurrencyDAL _currencyDal;
        public CurrencyBs(ICurrencyDAL currencyDal)
        {
            _currencyDal = currencyDal;
        }
        public List<Currency> GetAllCurrencies()
        {
            return _currencyDal.GetAllCurrencies();
        }
    }
}
