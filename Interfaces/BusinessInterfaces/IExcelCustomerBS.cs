using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Library.Entities;

namespace Interfaces.BusinessIntefaces
{
    public interface IExcelCustomerBS : IBaseBS
    {
        void CreateCustomers(List<ExcelToCustomerDto> lst);
    }
}
