using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.BusinessIntefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CurrencyController : ControllerBase
    {
        ICurrencyBS _currencyBS;
        public CurrencyController(ICurrencyBS currencyBS)
        {
            _currencyBS = currencyBS;
        }

        [HttpGet]
        [ActionName("GetAllCurrencies")]
        public IActionResult GetAllCurrencies()
        {
            var currencies = _currencyBS.GetAllCurrencies();
            return Ok(currencies);
        }

    }
}