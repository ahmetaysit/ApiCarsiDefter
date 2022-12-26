﻿using Core;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dto;
using Interfaces.Repository;

namespace Interfaces.DALINterfaces
{
    public interface ILogsDAL : IGenericRepository<Logs>, IBaseDAL
    {
    }
}
