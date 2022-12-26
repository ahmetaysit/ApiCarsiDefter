using DataLayer;
using DataLayer.Repository;
using Interfaces.DALINterfaces;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SmsRequestDAL : GenericRepository<SmsRequest> , ISmsRequestDAL
    {
        ProjectDbContext _db;
        public SmsRequestDAL(ProjectDbContext db)
            :base(db)
        {
            _db = db;
        }
    }
}
