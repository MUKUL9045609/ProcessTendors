using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessTendors.Application.Common.Models
{
    public class LoginRequest
    {   public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
