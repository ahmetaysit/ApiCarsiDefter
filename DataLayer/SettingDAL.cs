using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using DataLayer;
using DataLayer.Repository;

namespace DAL
{
    public class SettingDAL : GenericRepository<Settings>, ISettingsDAL
    {
        ProjectDbContext _db;
        public SettingDAL(ProjectDbContext db) : base(db)
        {
            _db = db;
        }
        public Settings Get(string key)
        {
            return _db.Settings.Where(x => x.SettingKey == key).FirstOrDefault();
        }
    }
}
