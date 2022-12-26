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
    public class CustomerGroupDAL : GenericRepository<CustomerGroup>, ICustomerGroupDAL
    {
        ProjectDbContext _db;
        public CustomerGroupDAL(ProjectDbContext db):base(db)
        {
            _db = db;
        }
        public CustomerGroup Get(int groupId)
        {
            return _db.CustomerGroup.Where(x => x.Id == groupId).FirstOrDefault();
        }

        public bool Save(CustomerGroup grp)
        {

            if (grp.Id > 0)
            {
                var existgrp = _db.CustomerGroup.Where(x => x.Id == grp.Id).FirstOrDefault();
                existgrp.GroupName = grp.GroupName;
                existgrp.IsActive = grp.IsActive;
            }
            else
                _db.CustomerGroup.Add(grp);
                        
            return _db.SaveChanges() > 0;
        }
    }
}
