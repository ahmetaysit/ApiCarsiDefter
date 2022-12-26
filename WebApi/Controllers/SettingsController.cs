using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Interfaces.BusinessIntefaces;
using Interfaces.BusinessInterfaces;
using Library.Dto;
using Library.Entities;
using Library.Enums;
using Library.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class SettingsController : ControllerBase
    {
        ICurrencyBS _currencyBS;
        IExcelCustomerBS _excelCustomerBS;
        ICustomerAccountBS _customerAccountBS;
        ICustomerBS _customerBS;
        ISmsRequestBS _smsRequestBS;
        ITransactionBS _transactionBS;
        ITransactionRequestBS _transactionRequestBS;
        IPoolIncomeFromGroupBS _poolIncomeFromGroupBS;
        IExcelTransactionBS _excelTransactionBS;
        ISettingsBS _settingsBS;
        public SettingsController(
            ICurrencyBS currencyBS,
            IExcelCustomerBS excelCustomerBS,
            ICustomerAccountBS customerAccountBS,
            ICustomerBS customerBS,
            ISmsRequestBS smsRequestBS,
            ITransactionBS transactionBS,
            IPoolIncomeFromGroupBS poolIncomeFromGroupBS,
            ITransactionRequestBS transactionRequestBS,
            IExcelTransactionBS excelTransactionBS,
            ISettingsBS settingsBS
            )
        {
            _currencyBS = currencyBS;
            _excelCustomerBS = excelCustomerBS;
            _customerAccountBS = customerAccountBS;
            _customerBS = customerBS;
            _smsRequestBS = smsRequestBS;
            _transactionBS = transactionBS;
            _poolIncomeFromGroupBS = poolIncomeFromGroupBS;
            _transactionRequestBS = transactionRequestBS;
            _excelTransactionBS = excelTransactionBS;
            _settingsBS = settingsBS;
        }

        [HttpPost]
        [ActionName("SaveBulkCustomer")]
        public IActionResult SaveBulkCustomer([FromBody]List<SaveBulkCustomerRequest> request)
        {
            List<Currency> currencies = _currencyBS.GetAllCurrencies();
            List<ExcelToCustomerDto> lst = new List<ExcelToCustomerDto>();
            foreach (var item in request)
            {

                ExcelToCustomerDto dto = new ExcelToCustomerDto();
                dto.TransactionDate = item.TransactionDate;
                dto.CustomerName = item.CustomerName;
                dto.CustomerGroupId = item.CustomerGroupId;
                dto.Email = item.Email;
                dto.PhoneNumber = item.PhoneNumber;
                dto.PoolRate = item.PoolRate;

                dto.Accounts = new List<ExcelCustomerAccountDto>();

                foreach (var currency in currencies)
                {
                    dto.Accounts.Add(new ExcelCustomerAccountDto { CurrencyId = currency.Id, Amount = (decimal)item.GetType().GetProperty(currency.CurrencyCode).GetValue(item, null) });
                }

                lst.Add(dto);
            }

            _excelCustomerBS.CreateCustomers(lst);

            return Ok("Ok");
        }

        [HttpPost]
        [ActionName("DeleteAllData")]
        public IActionResult DeleteAllData()
        {
            _customerAccountBS.DeleteAllAccounts();

            _customerBS.SaveCustomer(
                new Customer
                {
                    CustomerName = "Ana Havuz Hesap",
                    CustomerCode = "1",
                    DefaultCurrencyId = 1,
                    IsActive = true,
                    PoolRate = 0,
                    IsJustForBalance = false,
                }
                );
            _transactionBS.DeleteAllTransactions();
            _smsRequestBS.DeleteAll();
            _poolIncomeFromGroupBS.DeleteAll();
            _transactionRequestBS.DeleteAll();
            return Ok("Ok");
        }

        [HttpPost]
        [ActionName("ImportBulkTransaction")]
        public IActionResult AddAllData([FromBody]List<ExcelToTransactionDto> dtos)
        {
            _excelTransactionBS.AddAllData(dtos);
            return Ok("Ok");
        }

        [HttpPost]
        [ActionName("RewindTransactions")]
        public IActionResult RewindTransactions([FromBody]GetDashBoardReportsRequest request)
        {
            _transactionBS.RewindTransactions(request.StartDate.Date.AddDays(1));
            return Ok("Ok");
        }

        [HttpGet]
        [ActionName("GetMailSettings")]
        public IActionResult GetMailSettings()
        {
            var settings = _settingsBS.GetByFilter(x=> x.SettingKey == "ToEmail");
            return Ok(settings);
        }

        [HttpPost]
        [ActionName("SaveSetting")]
        public IActionResult SaveSetting([FromBody] Settings setting)
        {
            if (setting.Id > 0)
                _settingsBS.Update(setting);
            else
                _settingsBS.Insert(setting);

            var settings = _settingsBS.GetByFilter(x => x.SettingKey == "ToEmail");
            return Ok(settings);
        }
    }
}