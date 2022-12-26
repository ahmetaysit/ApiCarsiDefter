using Core;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.BusinessInterfaces
{
    public interface IExcelTransactionBS : IBaseBS
    {
        void AddAllData(List<ExcelToTransactionDto> dtos);
    }
}
