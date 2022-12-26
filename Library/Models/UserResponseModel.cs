using Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class UserResponseModel : User
    {
        public string Token { get; set; }

    }
}
