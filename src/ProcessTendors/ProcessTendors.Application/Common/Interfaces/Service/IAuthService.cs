using ProcessTendors.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ProcessTendors.Application.Common.Interfaces.Service
{
    public interface IAuthService
    {
        Task<Tuple<string, int>> GenerateToken(UserContextModel model);
        Task<string> GenerateRefreshToken();
    }
}
