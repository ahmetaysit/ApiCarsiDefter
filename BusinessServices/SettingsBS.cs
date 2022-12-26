using BusinessServices.Repository;
using Interfaces.BusinessInterfaces;
using Interfaces.DALINterfaces;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessServices
{
    public class SettingsBS : GenericRepositoryBs<ISettingsDAL, Settings>,ISettingsBS
    {
        ISettingsDAL _settingsDal;
        public SettingsBS(ISettingsDAL settingsDAL):base(settingsDAL)
        {
            _settingsDal = settingsDAL;
        }
        public Settings Get(string key)
        {
            return _settingsDal.Get(key);
        }
    }
}
