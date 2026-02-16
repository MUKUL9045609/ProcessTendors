
using ProcessTendors.Application.Common.Models;

namespace ProcessTendors.Application.Common.Interfaces.Service
{
    public interface IScraperService
    {
        Task<APIResponse> ProcessURL();
    }
}
