using Core;
using Library.Entities;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.DALINterfaces
{
    public interface ISmsRequestDAL :IGenericRepository<SmsRequest> , IBaseDAL
    {
    }
}
