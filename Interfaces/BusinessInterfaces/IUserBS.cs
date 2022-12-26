using Core;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.BusinessIntefaces
{
    public interface IUserBS : IBaseBS
    {
        User Login(string userName, string password);
    }
}
