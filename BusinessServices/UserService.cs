using AutoMapper;
using BusinessServices.Repository;
using Core;
using Interfaces;
using Interfaces.DalInterfaces;
using Interfaces.Repository;
using Library.Entities;
using Library.Helpers;
using Library.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessServices
{
    public class UserService : GenericRepositoryBs<IUserDal, User>, IUserService
    {
        private IUserDal _userDal;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        IUnitOfWork _unitOfWork;

        public UserService(IOptions<AppSettings> appSettings, IUserDal userDal, IMapper mapper, IUnitOfWork unitOfWork) : base(userDal)
        {
            _unitOfWork = unitOfWork;
            _userDal = userDal;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public User Authenticate(string username, string password)
        {
            string passwordHash = EncryptHelper.MD5Hash(password);
            var user = _userDal.GetByFilter(x => x.Username == username && x.Password == passwordHash && x.IsActive).SingleOrDefault();

            if (user == null)
            {
                return null;
            }

            UserResponseModel userResponseModel = _mapper.Map<UserResponseModel>(user.WithoutPassword());
            userResponseModel.Token = TokenHandler.GetTokenByUser(user, _appSettings);

            return userResponseModel;
        }

        //public IEnumerable<User> GetAll()
        //{
        //    return _userDal.GetAll().WithoutPasswords();
        //}

        public bool IsUserExists(string username)
        {
            return _userDal.GetByFilter(x => x.Username == username).FirstOrDefault() != null;
        }

        public User Register(string username, string password)
        {
            string passwordHash = EncryptHelper.MD5Hash(password);
            User user = new User { Username = username, Password = passwordHash, IsActive = true };

            _userDal.Insert(user);

            return Authenticate(username, password);
        }
    }
}
