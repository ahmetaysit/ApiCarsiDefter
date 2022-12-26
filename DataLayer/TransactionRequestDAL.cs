using DataLayer;
using Core;
using Interfaces.DALINterfaces;
using Library.Entities;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Helpers;
using Interfaces.DalInterfaces;
using DataLayer.Repository;
using Library.Responses;

namespace DAL
{
    public class TransactionRequestDAL : GenericRepository<TransactionRequest>, ITransactionRequestDAL
    {
        ProjectDbContext _db;
        public TransactionRequestDAL(ProjectDbContext db)
            : base(db)
        {
            _db = db;
        }

        public List<TransactionRequestListItem> GetAllTransacions()
        {
            var result = (from tranReq in _db.TransactionRequest
                          join cust in _db.Customer on tranReq.CustomerId equals cust.Id
                          from toAccount in _db.CustomerAccount.Where(x=> tranReq.ToAccountId == x.Id).DefaultIfEmpty()
                          from fromAccount in _db.CustomerAccount.Where(x => tranReq.FromAccountId == x.Id).DefaultIfEmpty()
                          //join fromAccount in _db.CustomerAccount on tranReq.FromAccountId equals fromAccount.Id
                          join user in _db.User on tranReq.CreatedBy equals user.Id
                          join tranReqType in _db.TransactionRequestType on tranReq.TransactionType equals tranReqType.TypeCode
                          join status in _db.TransactionRequestStatus on tranReq.Status equals status.StatusCode
                          where tranReq.Status == 1
                          select new TransactionRequestListItem
                          {
                              TransactionRequestId = tranReq.Id,
                              TranSactionType = tranReqType.TypeName,
                              TranSactionTypeCode = tranReq.TransactionType,
                              CustomerId = tranReq.CustomerId,
                              CustomerName = cust.CustomerName,
                              Amount = tranReq.Amount,
                              ToAccount = toAccount.AccountNo,
                              ToAccountId = tranReq.ToAccountId,
                              FromAccount = fromAccount.AccountNo,
                              FromAccountId = tranReq.FromAccountId,
                              BuyingRate = tranReq.BuyingRate,
                              SellingRate = tranReq.SellingRate,
                              Status = status.StatusName,
                              CreatedUser = user.NameSurname,
                              TransactionDate = tranReq.TransactionDate,
                              CreatedDate = tranReq.CreatedDate,
                              FromAccBalanceAfter = tranReq.FromAccBalanceAfter,
                              ToAccBalanceAfter = tranReq.ToAccBalanceAfter,
                              Description = tranReq.Description
                          }).ToList();

            return result;
        }
    }
}
