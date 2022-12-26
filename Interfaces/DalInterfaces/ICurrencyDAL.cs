using Core;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Repository;

namespace Interfaces.DALINterfaces
{
    public interface ICurrencyDAL : IBaseDAL
    {
        List<Currency> GetAllCurrencies();
    }
}
