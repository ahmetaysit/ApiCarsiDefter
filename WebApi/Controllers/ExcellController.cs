using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.BusinessIntefaces;
using Library.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    //[CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExcellController : ControllerBase
    {
        ICustomerAccountBS _customerAccountBS;
        public ExcellController(ICustomerAccountBS customerAccountBS)
        {
            _customerAccountBS = customerAccountBS;
        }

        [HttpPost]
        [ActionName("GenerateEndOfMonthExcel")]
        public IActionResult GenerateEndOfMonthExcel([FromBody]GenerateEndOfMonthExcelRequest request)
        {
            var res = _customerAccountBS.GenerateEndOfMonthExcel(request);
            return Ok(Convert.ToBase64String(res as byte[]));
        }

    }
}