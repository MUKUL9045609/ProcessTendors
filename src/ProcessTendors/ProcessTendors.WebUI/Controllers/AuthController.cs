using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using ProcessTendors.Application.Common.Interfaces.Repositories;
using ProcessTendors.Application.Common.Interfaces.Service;
using ProcessTendors.Application.Common.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProcessTendors.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuthRepositories _authRepositories;

        public AuthController(IAuthService authService,IAuthRepositories authRepositories)
        {
            _authService = authService;
            _authRepositories = authRepositories;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<APIResponse> Login(LoginRequest request)
        {
            try
            {
                var LoginDetails = await _authRepositories.ValidateCredentials(request);

                if (LoginDetails.Name is null)
                {
                    return new APIResponse()
                    {
                        Status = true,
                        Message = "Invalid Credentails",
                        data = new Data
                        {
                            Token = null,
                            ExpiryInSeconds = 0
                        }
                    };
                }
                var token = await _authService.GenerateToken(new UserContextModel()
                {
                    UserId = LoginDetails.UserId,
                    Name = LoginDetails.Name,
                    Role = LoginDetails.Role,
                });

                return new APIResponse()
                {
                    Status = true,
                    Message = "Success",
                    data = new Data
                    {
                        Token = token.Item1,
                        ExpiryInSeconds = token.Item2
                    }
                };

            }
            catch (Exception ex)
            {
                return new APIResponse()
                {
                    Status = true,
                    Message = "Technical issue",
                    data = new Data
                    {
                        Token = null,
                        ExpiryInSeconds = 0
                    }
                };
            }
        }
    }
}
