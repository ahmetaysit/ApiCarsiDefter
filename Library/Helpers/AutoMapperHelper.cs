using AutoMapper;
using Library.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Helpers
{
    public class AutoMapperHelper : Profile
    {
        //Mapper _mapper;
        public AutoMapperHelper():base()
        {
            var configuration = new MapperConfiguration(cfg => { });
            CreateMap<User, UserResponseModel>();
        }
    }
}
