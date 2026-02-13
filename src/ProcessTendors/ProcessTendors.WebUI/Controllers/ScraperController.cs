using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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
               //DataTable urls = LoadCsv("C:\\Users\\Fortune4\\Downloads\\urls.csv");


                var client = _clientFactory.CreateClient();

                var html = await client.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var node = doc.DocumentNode.SelectSingleNode("//body");
                var innerHtml = ProcessHtml(node?.InnerHtml);

                return Ok(new { InnerHtml = innerHtml });
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }


        public static string ComputeHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input.Trim());
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static DataTable LoadCsv(string path)
        {
            DataTable dt = new DataTable();

            using (var reader = new StreamReader(path))
            {
                string headerLine = reader.ReadLine();
                var headers = headerLine.Split(',');

                foreach (var h in headers)
                    dt.Columns.Add(h);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    dt.Rows.Add(line.Split(','));
                }
            }

            return dt;
        }

        public static string ProcessHtml(string body)
        {
            var removePatterns = new List<string> { "footer", "ads", "tracking" };
            var clonedBody = new HtmlAgilityPack.HtmlDocument();
            clonedBody.LoadHtml(body);

            var nodes = clonedBody.DocumentNode.Descendants().ToList();

            foreach (var node in nodes)
            {
                var classAttr = node.GetAttributeValue("class", null);
                if (classAttr != null)
                {
                    var classList = classAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (classList.Any(className => removePatterns.Any(pattern => className.Contains(pattern))))
                    {
                        node.Remove();
                    }
                }
            }

            return clonedBody.DocumentNode.InnerText;
        }
    }
}
