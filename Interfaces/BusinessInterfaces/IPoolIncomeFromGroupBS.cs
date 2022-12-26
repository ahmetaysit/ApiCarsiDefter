using Core;
using Library.Entities;
using Interfaces.Repository;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.BusinessIntefaces
{
    public interface IPoolIncomeFromGroupBS : IGenericRepository<PoolIncomeFromGroup>, IBaseBS
    {
        void AddDailyProfit(Transaction tran);
        void AddShopProfit(ShopProfitEntryDto dto,CustomerAccountDto customerAccount, int customerId);
    }
}
