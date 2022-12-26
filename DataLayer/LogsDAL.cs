using Interfaces.DALINterfaces;
using Core;
using Library.Entities;
using Library.Dto;
using Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Repository;

namespace DAL
{
    public class LogsDAL : GenericRepository<Logs>, ILogsDAL
    {
        ProjectDbContext _db;
        public LogsDAL(ProjectDbContext db):base(db)
        {
            _db = db;
        }
    }
}
