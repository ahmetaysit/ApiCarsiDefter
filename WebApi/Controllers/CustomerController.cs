using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Interfaces;
using Interfaces.BusinessIntefaces;
using Interfaces.BusinessInterfaces;
using Library.Dto;
using Library.Entities;
using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        ICustomerBS _customerBS;
        ICustomerAccountBS _customerAccountBS;
        ICustomerGroupBS _customerGroupBS;
        IEmailSenderBS _emailSenderBS;
        ICustomerRequestBS _customerRequestBS;
        public CustomerController(ICustomerBS customerBS, ICustomerAccountBS customerAccountBS, ICustomerGroupBS customerGroupBS, IEmailSenderBS emailSenderBS,ICustomerRequestBS customerRequestBS)
        {
            _customerBS = customerBS;
            _customerAccountBS = customerAccountBS;
            _customerGroupBS = customerGroupBS;
            _emailSenderBS = emailSenderBS;
            _customerRequestBS = customerRequestBS;
        }

        [HttpGet]
        [ActionName("GetAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            var customers = _customerBS.GetCustomers();
            return Ok(customers);
        }

        [HttpGet]
        [ActionName("GetCustomerById")]
        public IActionResult GetAllCustomers(int customerId)
        {
            var customers = _customerBS.Get(customerId);
            return Ok(customers);
        }

        [HttpGet]
        [ActionName("GetCustomerGroups")]
        public IActionResult GetCustomerGroups()
        {
            var customers = _customerBS.GetCustomerGroups();
            return Ok(customers);
        }

        [HttpGet]
        [ActionName("GetCustomerAllGroups")]
        public IActionResult GetCustomerAllGroups()
        {
            var groups = _customerGroupBS.GetAll();
            return Ok(groups);
        }

        [HttpPost]
        [ActionName("SaveCustomer")]
        public IActionResult SaveCustomer([FromBody]Customer customer)
        {
            var customers = _customerBS.SaveCustomer(customer);
            return Ok(customers);
        }

        [HttpPost]
        [ActionName("CreateCustomerWithBalance")]
        public IActionResult CreateCustomerWithBalance([FromBody] AddCustomerSummaryDto customer)
        {
            var customers = _customerBS.CreateCustomerWithBalance(customer);
            return Ok(customers);
        }

        [HttpGet]
        [ActionName("GetCustomerRequestList")]
        public IActionResult GetCustomerRequestList()
        {
            var result = _customerRequestBS.GetByFilter(x => x.Status == 1).ToList();
            return Ok(result);
        }

        [HttpPost]
        [ActionName("ApproveCustomerRequest")]
        public IActionResult ApproveCustomerRequest(int id)
        {
            _customerBS.ApproveCustomerRequest(id);

            var result = _customerRequestBS.GetByFilter(x => x.Status == 1).ToList();
            return Ok(result);
        }

        [HttpPost]
        [ActionName("RejectCustomerRequest")]
        public IActionResult RejectCustomerRequest(int id)
        {
            _customerBS.RejectCustomerRequest(id);
            var result = _customerRequestBS.GetByFilter(x => x.Status == 1).ToList();
            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetCustomerAccounts")]
        public IActionResult GetCustomerAccounts(int customerId)
        {
            var customerAccounts = _customerBS.GetCustomerAccounts(customerId);
            return Ok(customerAccounts);
        }

        [HttpPost]
        [ActionName("ExchangeTransfer")]
        public IActionResult ExchangeTransfer([FromBody]CustomerExchangeTransferDto dto)
        {
            var context = User.FindFirst("userContext");
            if (context != null)
            {
                var user = JsonConvert.DeserializeObject<UserResponseModel>(context.Value);
                dto.UserName = user.NameSurname;
            }
            
            dto.TransactionDate = dto.TransactionDate.Date;
            var result = _customerAccountBS.ExchangeTransfer(dto);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("ExchangeTransferWithRate")]
        public IActionResult ExchangeTransferWithRate([FromBody] CustomerExchangeTransferDto dto)
        {
            var context = User.FindFirst("userContext");
            if (context != null)
            {
                var user = JsonConvert.DeserializeObject<UserResponseModel>(context.Value);
                dto.UserName = user.NameSurname;
            }

            dto.TransactionDate = dto.TransactionDate.Date;
            var result = _customerAccountBS.ExchangeTransfer(dto);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("MakeMonthEndProcess")]
        public IActionResult MakeMonthEndProcess([FromBody]EnOfMonthTransactionDto dto)
        {
            var result = _customerAccountBS.MakeMonthEndProcess(dto.SelectedDate.Date);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("SaveCustomerGroup")]
        public IActionResult SaveCustomerGroup([FromBody]CustomerGroup entity)
        {
            _customerGroupBS.Save(entity);
            var result = _customerGroupBS.GetAll();
            return Ok(result);
        }
    }
}