using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Interfaces.BusinessIntefaces;
using Interfaces.BusinessInterfaces;
using Interfaces.DalInterfaces;
using Library.Dto;
using Library.Entities;
using Library.Enums;
using Library.Models;
using Library.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TransactionController : ControllerBase
    {
        ITransactionBS _transactionBS;
        ITransactionRequestBS _transactionRequestBS;
        ICustomerAccountBS _customerAccountBS;
        ICustomerBS _customerBS;
        ISettingsBS _settingsBS;
        IEmailSenderBS _emailSenderBS;
        IUserDal _userDal;
        IReportsBS _reportsBS;
        public TransactionController(
            ITransactionBS transactionBS, 
            ITransactionRequestBS transactionRequestBS, 
            ICustomerAccountBS customerAccountBS,
            ICustomerBS customerBS, 
            ISettingsBS settingsBS,
            IEmailSenderBS emailSenderBS,
            IUserDal userDal,
            IReportsBS reportsBS)
        {
            _transactionBS = transactionBS;
            _transactionRequestBS = transactionRequestBS;
            _customerAccountBS = customerAccountBS;
            _customerBS = customerBS;
            _settingsBS = settingsBS;
            _emailSenderBS = emailSenderBS;
            _userDal = userDal;
            _reportsBS = reportsBS;
        }

        [HttpPost]
        [ActionName("AddBulkTransaction")]
        public IActionResult AddBulkTransaction([FromBody]BulkTransactionDto bulkTransactionDto)
        {
            _transactionBS.AddBulk(bulkTransactionDto);
            DateTime selectedDate = bulkTransactionDto.TransactionDate.Date;
            DateTime startDate = new DateTime(selectedDate.Year, selectedDate.Month-1, 1);
            List<CustomerTransactionHistoryDto> source = _transactionBS.GetDayProfitHistory(startDate, selectedDate);
            var report = _reportsBS.GetGeneralTransactionHistory(bulkTransactionDto.TransactionDate.Date, bulkTransactionDto.TransactionDate.Date);
            MailMessage mail = CreateMailForBulkTran(bulkTransactionDto, report);

            _emailSenderBS.SendEmail(mail);

            return Ok(source);
        }


        [HttpPost]
        [ActionName("AddShopProfit")]
        public IActionResult AddShopProfit([FromBody]AddShopProfitDto dto)
        {
            int poolCustomerId = Convert.ToInt32(_settingsBS.Get("PoolCustomer").Settingvalue);
            var allCustomerAccounts = _customerBS.GetAllCustomerAccounts();
            var allTransactions = _transactionBS.GetByFilter(x => x.TransactionDate <= dto.SelectedDate && x.IsShopProfitTaken == false).ToList();
            Dictionary<Transaction, DateTime> dic = new Dictionary<Transaction, DateTime>();

            foreach (var currency in dto.SelectedCurrencies)
            {
                ShopProfitEntryDto profitEntryDto = new ShopProfitEntryDto
                {
                    CurrencyId = currency.Id,
                    CustomerIds = dto.SelectedCustomers.Select(x => x.Id).ToList(),
                    DayCount = dto.DayCount,
                    TransactionDate = dto.SelectedDate.Date,
                    ProfitAmount = dto.TotalProfit,
                    TotalAmount = dto.TotalAmount
                };
                _transactionBS.AddShopProfit(profitEntryDto, poolCustomerId, allCustomerAccounts, allTransactions,dic);
            }

            _transactionBS.Save();

            foreach (var item in dic)
            {
                var shopTrans = allTransactions.Where(x => x.TransactionDate <= item.Value && x.IsShopProfitTaken == false && x.CustomerAccountId == item.Key.CustomerAccountId).ToList();
                foreach (var tran in shopTrans)
                {
                    tran.IsShopProfitTaken = true;
                    tran.ShopProfitTransactionId = item.Key.Id;
                    _transactionBS.Update(tran, true);
                }
            }
            _transactionBS.Save();

            return Ok(true);
        }

        [HttpPost]
        [ActionName("AddTransactionRequest")]
        public IActionResult AddTransactionRequest([FromBody]TransactionRequest request)
        {
            request.TransactionDate = request.TransactionDate.Date;
            var accounts = _customerBS.GetCustomerAccounts(request.CustomerId);
            var fromAcc = accounts.Where(x => x.Id == request.FromAccountId).FirstOrDefault();
            var toAcc = accounts.Where(x => x.Id == request.ToAccountId).FirstOrDefault();
            request.FromAccBalanceAfter = request.FromAccountId > 0 ? fromAcc.CalculatedLastAccountBalance - request.Amount : 0;
            request.ToAccBalanceAfter = request.ToAccountId > 0 ? toAcc.CalculatedLastAccountBalance + request.Amount / request.BuyingRate * request.SellingRate : 0;
            _transactionRequestBS.Insert(request);
            var hasToApproveSetting = _settingsBS.GetByFilter(x => x.SettingKey == "HasToApprove").FirstOrDefault();
            if(hasToApproveSetting != null && hasToApproveSetting.Settingvalue == "0")
            {
                var reqItem = new TransactionRequestListItem
                {
                    Amount = request.Amount,
                    BuyingRate = request.BuyingRate,
                    SellingRate = request.SellingRate,
                    CustomerId = request.CustomerId,
                    FromAccountId = request.FromAccountId,
                    ToAccountId = request.ToAccountId,
                    TransactionDate = request.TransactionDate.Date,
                    TranSactionTypeCode = request.TransactionType,
                    Description = request.Description
                };
                _transactionRequestBS.ChangeStatus(reqItem,2,request.CreatedBy);
            }
            MailMessage eMail = CreateMailMessageForRequest(request, fromAcc, toAcc);
            _emailSenderBS.SendEmail(eMail);

            return Ok(true);
        }

        [HttpGet]
        [ActionName("GetTransactionRequests")]
        public IActionResult GetTransactionRequests(int customerId)
        {
            var result = _transactionRequestBS.GetAllTransacions().OrderByDescending(x => x.CreatedDate).ToList();

            if (customerId > 0)
                result = result.Where(x => x.CustomerId == customerId).ToList();

            return Ok(result);
        }

        [HttpPost]
        [ActionName("ApproveTransactionRequest")]
        public IActionResult ApproveTransactionRequest([FromBody]TransactionRequestListItem request)
        {
            int userId = Convert.ToInt32(User.Identity.Name);
            _transactionRequestBS.ChangeStatus(request, 2, userId);
            var result = _transactionRequestBS.GetAllTransacions().OrderByDescending(x => x.CreatedDate).ToList();
            MailMessage eMail = CreateMailMessageForApproveReject(request, "Onaylandı");
            _emailSenderBS.SendEmail(eMail);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("RejectTransactionRequest")]
        public IActionResult RejectTransactionRequest([FromBody]TransactionRequestListItem request)
        {
            int userId = Convert.ToInt32(User.Identity.Name);
            _transactionRequestBS.ChangeStatus(request, 3, userId);
            var result = _transactionRequestBS.GetAllTransacions().OrderByDescending(x => x.CreatedDate).ToList();
            MailMessage eMail = CreateMailMessageForApproveReject(request, "Reddedildi");
            _emailSenderBS.SendEmail(eMail);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetMaxTransactionDate")]
        public IActionResult GetMaxTransactionDate()
        {
            var result = _transactionBS.GetMaxTransactionDate();
            return Ok(result);
        }


        private MailMessage CreateMailMessageForRequest(TransactionRequest request, CustomerAccountDto fromAcc, CustomerAccountDto toAcc)
        {
            User createdUser = _userDal.GetById(request.CreatedBy);
            var customer = _customerBS.Get(request.CustomerId);
            MailMessage eMail = new MailMessage();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{createdUser.NameSurname} tarafından aşağıda ayrıntıları verilen İşlem İsteği gönderildi");
            sb.AppendLine($"Müşteri Adı = {customer.CustomerName}");
            sb.AppendLine($"Tarih = {request.TransactionDate.ToShortDateString()}");
            if (request.TransactionType == 3) sb.AppendLine($"Çıkış Hesabı = {fromAcc.AccountNo} Giriş Hesabı = {toAcc.AccountNo}");
            if (request.TransactionType == 1) sb.AppendLine($"Giriş Hesabı = {toAcc.AccountNo}");
            if (request.TransactionType == 2) sb.AppendLine($"Çıkış Hesabı = {fromAcc.AccountNo}");
            sb.AppendLine($"Alış Kuru = {request.BuyingRate} Satış Kuru = {request.SellingRate} Tutar = {request.Amount}");
            eMail.Subject = $"{createdUser.NameSurname} tarafından İşlem İsteği gönderildi";
            eMail.Body = sb.ToString();
            return eMail;
        }
        private MailMessage CreateMailMessageForApproveReject(TransactionRequestListItem request, string tranType)
        {
            var customer = _customerBS.Get(request.CustomerId);
            MailMessage eMail = new MailMessage();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{request.CreatedUser} tarafından aşağıda ayrıntıları verilen İşlem İsteği {tranType}");
            sb.AppendLine($"Müşteri Adı = {customer.CustomerName}");
            sb.AppendLine($"Tarih = {request.TransactionDate.ToShortDateString()}");
            if (request.TranSactionTypeCode == 3) sb.AppendLine($"Çıkış Hesabı = {request.FromAccount} Giriş Hesabı = {request.ToAccount}");
            if (request.TranSactionTypeCode == 1) sb.AppendLine($"Giriş Hesabı = {request.ToAccount}");
            if (request.TranSactionTypeCode == 2) sb.AppendLine($"Çıkış Hesabı = {request.FromAccount}");
            sb.AppendLine($"Alış Kuru = {request.BuyingRate} Satış Kuru = {request.SellingRate} Tutar = {request.Amount}");
            eMail.Subject = $"{request.CreatedUser} tarafından İşlem İsteği {tranType}";
            eMail.Body = sb.ToString();
            return eMail;
        }
        private MailMessage CreateMailForBulkTran(BulkTransactionDto bulkTransactionDto, List<CustomerTransactionHistoryDtoExtention> report)
        {
            MailMessage mail = new MailMessage();
            mail.Subject = $"{bulkTransactionDto.TransactionDate.ToShortDateString()} Tarihli Gün Sonu Özeti";

            StringBuilder sb = new StringBuilder();
            string allHtml = @"<html>
                    <head>
                    <style>
                    table, td, th {
                      border: 1px solid black;
                    }

                    table {
                      border-collapse: collapse;
                    }
                    </style>
                    </head>
                    <body>##TABLE</body> </html>
                    ";
            sb.AppendLine("");
            sb.AppendLine("<table> <tr><th>İşlem Türü</th><th>Miktar</th><th>İsim</th><th>Hesap Bilgisi</th><th>Açıklama</th></tr> ");
            foreach (CustomerTransactionHistoryDto item in report)
            {
                if (string.IsNullOrEmpty(item.AccountNo))
                    //sb.AppendLine($"Gün Sonu Kar girişi Alış Kuru {item.BuyingRate} Satış Kuru {item.SellingRate} ");
                sb.AppendLine($"<tr><td  colspan='4'>Gün Sonu Kar girişi Alış Kuru {item.BuyingRate} Satış Kuru {item.SellingRate} - {item.CurrencyCode}</td></tr> ");
                else
                    sb.AppendLine($"<tr><td>{item.TransactionType}</td><td>{item.TransactionAmount}</td><td>{item.CustomerName}</td><td>{item.AccountNo}</td><td>{item.TransactionDescription}</td></tr> ");
                //sb.AppendLine($"{item.TransactionType} - {item.TransactionAmount} - {item.CustomerName} - {item.AccountNo}");
            }
            sb.AppendLine("</table>");

            string html = allHtml.Replace("##TABLE", sb.ToString());

            mail.Body = html;
            mail.IsBodyHtml = true;
            return mail;
        }


    }
}