using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool CantSeeBalance { get; set; }
    }
}
