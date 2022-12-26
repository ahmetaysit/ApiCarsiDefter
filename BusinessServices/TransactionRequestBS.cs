using BusinessServices.Repository;
using Interfaces.BusinessIntefaces;
using Interfaces.BusinessInterfaces;
using Interfaces.DalInterfaces;
using Library.Dto;
using Library.Entities;
using Library.Enums;
using Library.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessServices
{
    public class TransactionRequestBS : GenericRepositoryBs<ITransactionRequestDAL, TransactionRequest>, ITransactionRequestBS
    {
        ITransactionRequestDAL _transactionRequestDAL;
        ICustomerAccountBS _customerAccountBS;

        public TransactionRequestBS(ITransactionRequestDAL transactionRequestDAL, ICustomerAccountBS customerAccountBS)
            : base(transactionRequestDAL)
        {
            _transactionRequestDAL = transactionRequestDAL;
            _customerAccountBS = customerAccountBS;
        }

        public List<TransactionRequestListItem> ChangeStatus(TransactionRequestListItem request, int newStatus, int userId)
        {
            if(newStatus == 2)
            {
                CustomerExchangeTransferDto dto = new CustomerExchangeTransferDto
                {
                    Amount = request.Amount,
                    CurrencyBuyingRate = request.BuyingRate,
                    CurrencySellingRate = request.SellingRate,
                    CustomerId = request.CustomerId,
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    TransactionDate = request.TransactionDate.Date,
                    IsEndOfMonth = false,
                    TransactionType = (ExchangeConversionType)request.TranSactionTypeCode,
                    Description = request.Description
                };
                _customerAccountBS.ExchangeTransfer(dto);
            }

            var entity = _transactionRequestDAL.GetById(request.TransactionRequestId);
            entity.Status = newStatus;
            entity.UpdatedDate = DateTime.Now;

            _transactionRequestDAL.Update(entity);

            return GetAllTransacions();
        }

        public List<TransactionRequestListItem> GetAllTransacions()
        {
            return _transactionRequestDAL.GetAllTransacions();
        }
    }
}
