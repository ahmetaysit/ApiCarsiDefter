using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.DalInterfaces
{
    public interface ILogger
    {
        void Log(string message, string methodName);
    }
}
