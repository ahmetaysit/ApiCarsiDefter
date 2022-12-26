using Interfaces.BusinessIntefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Library.Entities;
using Interfaces.DALINterfaces;
using BusinessServices.Repository;

namespace BusinessServices
{
    public class SmsRequestBS : GenericRepositoryBs<ISmsRequestDAL,SmsRequest>, ISmsRequestBS
    {
        ISmsRequestDAL _smsRequestDal;
        public SmsRequestBS(ISmsRequestDAL smsRequestDal)
            :base(smsRequestDal)
        {
            _smsRequestDal = smsRequestDal;
        }

    }
}
