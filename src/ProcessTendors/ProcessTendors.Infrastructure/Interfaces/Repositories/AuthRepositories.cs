using ProcessTendors.Application.Common.Interfaces.Repositories;
using ProcessTendors.Application.Common.Interfaces.Service;
using ProcessTendors.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessTendors.Infrastructure.Interfaces.Repositories
{
    public class AuthRepositories : IAuthRepositories
    {
        public AuthRepositories() {

        }
        public async Task<LoginResponse> ValidateCredentials(LoginRequest model)
        {
            try
            {
                LoginResponse response = new LoginResponse();

                if (model.UserName == "Admin" && model.Password =="Admin")
                {
                   
                    response.Role = "Admin";
                    response.Name = "Admin";
                    response.UserId = 1;

                }

              
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
