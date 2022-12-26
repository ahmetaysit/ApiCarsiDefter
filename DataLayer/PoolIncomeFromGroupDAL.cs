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
    public class PoolIncomeFromGroupDAL : GenericRepository<PoolIncomeFromGroup>, IPoolIncomeFromGroupDAL
    {
        ProjectDbContext _db;
        public PoolIncomeFromGroupDAL(ProjectDbContext db) : base(db)
        {
            _db = db;

        }

        public void Add(PoolIncomeFromGroup entity)
        {
            _db.PoolIncomeFromGroup.Add(entity);
        }
    }
}
