using Core;
using Interfaces.Repository;
using Library.Entities;
using Library.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.BusinessInterfaces
{
    public interface ITransactionRequestBS : IGenericRepository<TransactionRequest>, IBaseBS
    {
        List<TransactionRequestListItem> GetAllTransacions();

        List<TransactionRequestListItem> ChangeStatus(TransactionRequestListItem request,int newStatus,int userId);
    }
}
