using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces;
using Interfaces.BusinessIntefaces;
using Library.Dto;
using Library.Entities;
using Library.Requests;
using Library.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportController : ControllerBase
    {
        IReportsBS _reportsBS;
        ISmsRequestBS _smsRequestBS;
        ICustomerBS _customerBS;
        public ReportController(IReportsBS reportsBS, ISmsRequestBS smsRequestBS, ICustomerBS customerBS)
        {
            _reportsBS = reportsBS;
            _customerBS = customerBS;
            _smsRequestBS = smsRequestBS;
        }

        [HttpPost]
        [ActionName("GetDashBoardReports")]
        public IActionResult GetDashBoardReports([FromBody]GetDashBoardReportsRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            var source = _reportsBS.ImmediateStateSummary(startDate, endDate);
            var source2 = _reportsBS.ImmediateStateGroupSummary(startDate, endDate);
            List<ImmediateStateDailyProfitDto> source3 = _reportsBS.ImmediateStateDailyProfit(startDate, endDate);

            decimal? tlTotalProfit = 1;
            decimal? usdTotalProfit = 1;
            decimal? eurTotalProfit = 1;
            decimal? gauTotalProfit = 1;
            foreach (ImmediateStateDailyProfitDto item in source3)
            {
                tlTotalProfit *= item.TLProfit;
                item.TLTotalProfit = tlTotalProfit;
                usdTotalProfit *= item.USDProfit;
                item.USDTotalProfit = usdTotalProfit;
                eurTotalProfit *= item.EURProfit;
                item.EURTotalProfit = eurTotalProfit;
                gauTotalProfit *= item.GAUProfit;
                item.GAUTotalProfit = gauTotalProfit;
            }
            GetDashBoardReportsResponse response = new GetDashBoardReportsResponse();
            response.ImmediateStateSummary = source;
            response.ImmediateStateGroupSummary = source2;
            response.ImmediateStateDailyProfit = source3;

            return Ok(response);
        }

        [HttpPost]
        [ActionName("GetGroupSummaryReport")]
        public IActionResult GetGroupSummaryReport([FromBody]GetDashBoardReportsRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            var response = _reportsBS.GetGroupSummaryReport(startDate, endDate);

            return Ok(response);
        }

        [HttpPost]
        [ActionName("GetCustomerTransactionHistory")]
        public IActionResult GetCustomerTransactionHistory([FromBody]GetCustomerTransactionHistoryRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;
            int customerId = 0;

            if (request.Customer != null)
            {
                customerId = request.Customer.Id;
            }

            var response = _reportsBS.GetCustomerTransactionHistory(customerId, startDate, endDate);

            return Ok(response);
        }

        [HttpPost]
        [ActionName("GetSmsRequests")]
        public IActionResult GetSmsRequests([FromBody] GetDashBoardReportsRequest request)
        {
            var source = _smsRequestBS.GetByFilter(x => x.CreationDate == request.StartDate.Date);
            var customer = _customerBS.GetCustomers();
            var source3 = (from req in source
                           from cust in customer.Where(cust => req.CustomerId == cust.Id).DefaultIfEmpty()
                           select new
                           {
                               cust.Id,
                               cust.CustomerName,
                               req.PhoneNumber,
                               req.SmsText,
                               req.CreationDate
                           }).OrderBy(x => x.Id).ToList();

            return Ok(source3);
        }


        [HttpPost]
        [ActionName("GetGeneralTransactionHistory")]
        public IActionResult GetGeneralTransactionHistory([FromBody]GetDashBoardReportsRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            var result = _reportsBS.GetGeneralTransactionHistory(startDate, endDate);

            return Ok(result);
        }

        [HttpPost]
        [ActionName("GetDayProfitHistoryWithCumulative")]
        public IActionResult GetDayProfitHistoryWithCumulative([FromBody] DateTime requestDate)
        {
            DateTime selectedDate = requestDate.Date;
            DateTime startDate = new DateTime(selectedDate.Year, selectedDate.Month - 1, 1);
            DateTime endDate = new DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month));

            GetDashBoardReportsRequest request = new GetDashBoardReportsRequest { StartDate = startDate.Date ,EndDate = endDate };

            var result = _reportsBS.GetDayProfitHistoryWithCumulative(request);

            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetLastFiftyTransaction")]
        public IActionResult GetLastFiftyTransaction()
        {
            var result = _reportsBS.GetLast50Transaction();

            return Ok(result);
        }
        

        [HttpGet]
        [ActionName("GetCustomerBackup")]
        public IActionResult GetCustomerBackup()
        {
            var result = _reportsBS.GetCustomerBackup();

            return Ok(result);
        }

        [HttpPost]
        [ActionName("GetMonthlyProfitHistory")]
        public IActionResult GetMonthlyProfitHistory([FromBody] GetDashBoardReportsRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            var result = _reportsBS.GetMonthlyProfitHistory(startDate, endDate);

            return Ok(result);
        }

    }
}