using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProcessTendors.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ScraperController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        
        [AllowAnonymous]
        [HttpGet("get-inner-html")]
        public async Task<IActionResult> GetInnerHtml(string url)
        {
            try
            {
                var client = _clientFactory.CreateClient();

                var html = await client.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var node = doc.DocumentNode.SelectSingleNode("//article");
                var innerHtml = node?.InnerHtml;

                return Ok(new { InnerHtml = innerHtml });
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }
    }
}
