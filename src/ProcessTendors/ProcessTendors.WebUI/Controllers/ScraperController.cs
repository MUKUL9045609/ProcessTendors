using CsvHelper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProcessTendors.Application.Common.Models;
using ProcessTendors.Domain.Helpers;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;

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

        [HttpGet("ProcessUrl")]
        public async Task<ActionResult> ProcessUrl()
        {
            string csvPath = "C:\\Users\\Fortune4\\Downloads\\urls.csv";

            try
            {
                DataTable dt = FileHelper.LoadCsv(csvPath);

                foreach (DataRow r in dt.Rows)
                {
                    string rowUrl = (r["URL"]?.ToString() ?? "").Trim();

                    var client = _clientFactory.CreateClient();

                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    client = new HttpClient(clientHandler);

                    var html = await client.GetStringAsync(rowUrl);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var node = doc.DocumentNode.SelectSingleNode("//body");

                    string newHash = HashHelper.ComputeHash(HtmlPageHelper.ProcessHtml(node?.InnerHtml));

                    UpdateRowStatus(dt, rowUrl, newHash);

                    SaveCsv(dt, csvPath);
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public static void UpdateRowStatus(DataTable dt, string url, string newHash)
        {
            foreach (DataRow r in dt.Rows)
            {
                string rowUrl = (r["URL"]?.ToString() ?? "").Trim();

                if (string.Equals(rowUrl, url, StringComparison.OrdinalIgnoreCase))
                {
                    string oldHash = (r["StoredHash"]?.ToString() ?? "").Trim();

                    if (string.IsNullOrEmpty(oldHash))
                    {
                        r["StoredHash"] = newHash;
                        r["LastUpdated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        r["Status"] = "FIRST_TIME";
                    }
                    else if (!string.Equals(oldHash, newHash, StringComparison.OrdinalIgnoreCase))
                    {
                        r["StoredHash"] = newHash;
                        r["LastUpdated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        r["Status"] = "UPDATED";
                    }
                    else
                    {
                        r["Status"] = "NO_CHANGE";
                    }

                    return;
                }
            }
        }

        public static void SaveCsv(DataTable dt, string path)
        {
            var lines = new List<string>();

            string header = string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            lines.Add(header);

            foreach (DataRow row in dt.Rows)
            {
                lines.Add(string.Join(",", row.ItemArray));
            }

            System.IO.File.WriteAllLines(path, lines);
        }

        public static void SaveCsv(List<UrlRecord> list, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(list);
            }
        }

    }
}
