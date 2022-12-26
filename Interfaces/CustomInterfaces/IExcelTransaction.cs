using Core;
using Library.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.CustomInterfaces
{
    public interface IExcelTransaction : ICustomBaseBS
    {
        void ExecuteTransaction(ExcelToTransactionDto dto);
    }
}
