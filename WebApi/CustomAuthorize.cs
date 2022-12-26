using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        public CustomAuthorize():base()
        {
        }

        public CustomAuthorize(string policy) : base(policy)
        {
        }
    }
}
