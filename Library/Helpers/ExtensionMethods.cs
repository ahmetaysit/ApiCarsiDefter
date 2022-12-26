using Library.Entities;
using Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Library.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            user.Password = null;
            return user;
        }
        public static UserResponseModel GetUser(this ClaimsPrincipal user)
        {
            return JsonConvert.DeserializeObject<UserResponseModel>(user.FindFirst("userContext").Value);
        }
    }
}
