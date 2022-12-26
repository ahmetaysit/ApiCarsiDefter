using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using DataLayer.Repository;
using DataLayer;

namespace DAL
{
    public class CustomerRequestDAL : GenericRepository<CustomerRequest>, ICustomerRequestDAL
    {
        ProjectDbContext _db;
        public CustomerRequestDAL(ProjectDbContext db):base(db)
        {
            _db = db;
        }
    }
}
