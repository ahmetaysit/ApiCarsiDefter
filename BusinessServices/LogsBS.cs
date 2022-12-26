using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Library.Enums;
using Library.Entities;
using Interfaces.DALINterfaces;
using BusinessServices.Repository;

namespace BusinessServices
{
    public class LogsBS : GenericRepositoryBs<ILogsDAL, Logs>, ILogsBS
    {
        private ILogsDAL _logsDAL;

        public LogsBS(ILogsDAL logsDAL) : base(logsDAL)
        {
            _logsDAL = logsDAL;
        }

    }
}
