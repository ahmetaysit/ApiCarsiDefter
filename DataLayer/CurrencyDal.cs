using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Library.Entities;
using DataLayer;

namespace DAL
{
    public class CurrencyDal : ICurrencyDAL
    {
        ProjectDbContext _db;
        public CurrencyDal(ProjectDbContext db)
        {
            _db = db;
        }
        public List<Currency> GetAllCurrencies()
        {
            return _db.Currency.ToList();
        }
    }
}