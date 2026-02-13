using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProcessTendors.Domain.Helpers;
using System.Data;

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
        
        [HttpGet("get-inner-html")]
        public async Task<IActionResult> GetInnerHtml(string url)
        {
            try
            {
                DataTable urls = CsvHelper.LoadCsv("C:\\Users\\Fortune4\\Downloads\\urls.csv");

                var client = _clientFactory.CreateClient();
                var html = await client.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var node = doc.DocumentNode.SelectSingleNode("//body");
                var innerHtml = HtmlPageHelper.ProcessHtml(node?.InnerHtml);

                return Ok(new { InnerHtml = innerHtml });
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }



    }
}
