using Core;
using Interfaces.Repository;
using Library.Entities;
using Library.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.DalInterfaces
{
    public interface ITransactionRequestDAL : IBaseDAL, IGenericRepository<TransactionRequest>
    {
        List<TransactionRequestListItem> GetAllTransacions();
    }
}
