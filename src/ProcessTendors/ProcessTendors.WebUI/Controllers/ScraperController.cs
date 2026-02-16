using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessTendors.Application.Common.Interfaces.Service;

namespace ProcessTendors.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : Controller
    {
        private readonly IScraperService _scraperService;

        public ScraperController(IScraperService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ProcessUrl()
        {
            try
            {
               var result = await _scraperService.ProcessURL();

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        

    }
}
