using ProcessTendors.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessTendors.Application.Common.Interfaces.Repositories
{
    public interface IAuthRepositories
    {
        Task<LoginResponse> ValidateCredentials(LoginRequest model);
    }
}
