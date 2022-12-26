using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Interfaces;
using Library.Models;
using System.Security.Claims;
using Library;
using System;
using Library.Entities;
using Library.Helpers;

namespace WebApi.Controllers
{
    //[Authorize]
    [CustomAuthorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [ActionName("authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);
            BaseResponse response = new BaseResponse { ResponseCode = 200, ResponseMessage = "Ok", ResponseData = user };
            if (user == null)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Kullanıcı adı ve şifre yanlış!";
                return Ok(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [ActionName("Register")]
        [HttpPost]
        public IActionResult Register([FromBody]AuthenticateModel model)
        {
            BaseResponse response = new BaseResponse { ResponseCode = 200, ResponseMessage = "Ok", ResponseData = null };
            if (_userService.IsUserExists(model.Username))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Kullanıcı adı zaten kullanılıyor !";
                return Ok(response);
            }

            var user = _userService.Register(model.Username, model.Password);

            if (user == null)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Kullanıcı adı ve şifre yanlış!";
                return Ok(response);
            }

            response.ResponseData = user;
            return Ok(response);
        }

        [ActionName("SaveUser")]
        [HttpPost]
        public IActionResult SaveUser([FromBody]User model)
        {
            BaseResponse response = new BaseResponse { ResponseCode = 200, ResponseMessage = "Ok", ResponseData = null };

            if (model.Id == 0 && _userService.IsUserExists(model.Username))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Kullanıcı adı zaten kullanılıyor !";
                return Ok(response);
            }

            if (model.Id > 0)
            {
                _userService.Update(model);
            }
            else
            {
                model.Password = EncryptHelper.MD5Hash("a");

                _userService.Insert(model);
            }

            response.ResponseData = _userService.GetAll();
            return Ok(response);
        }

        [ActionName("SaveMyProfile")]
        [HttpPost]
        public IActionResult SaveMyProfile([FromBody]SaveProfileRequestModel model)
        {
            BaseResponse response = new BaseResponse { ResponseCode = 200, ResponseMessage = "Ok", ResponseData = null };

            var user = _userService.GetById(model.Id);

            user.NameSurname = model.NameSurname;
            user.Username = model.UserName;
            user.Password = EncryptHelper.MD5Hash(model.Password);
            _userService.Update(user);

            return Ok(response);
        }

        [HttpGet]
        [ActionName("GetUser")]
        public IActionResult GetUser()
        {
            int userId = Convert.ToInt32(User.Identity.Name);
            var user = _userService.GetAll().Where(x => x.Id == userId).First();
            return Ok(user);
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}