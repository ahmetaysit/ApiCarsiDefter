using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Repository
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }

}
