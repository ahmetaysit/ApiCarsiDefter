using DataLayer.Repository;
using Interfaces.DalInterfaces;
using Interfaces.Repository;
using Library.Entities;
using System;
using System.Collections.Generic;

namespace DataLayer
{
    public class UserDal : GenericRepository<User>, IUserDal
    {
        public UserDal(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
