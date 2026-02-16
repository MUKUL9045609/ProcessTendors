using CsvHelper;
using HtmlAgilityPack;
using ProcessTendors.Application.Common.Interfaces.Service;
using ProcessTendors.Application.Common.Models;
using ProcessTendors.Domain.Helpers;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProcessTendors.Infrastructure.Interfaces
{
    public class ScraperService : IScraperService
    {
        private readonly IHttpClientFactory _clientFactory;


        public ScraperService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<APIResponse> ProcessURL()
        {
            try
            {
                string csvPath = "C:\\Users\\Fortune4\\Downloads\\urls.csv";

                DataTable dt_original = FileHelper.LoadCsv(csvPath);

                DataTable dt_processed = dt_original.Copy();

                foreach (DataRow r in dt_processed.Rows)
                {
                    string rowUrl = (r["URL"]?.ToString() ?? "").Trim();
                    string newHash = "NA";

                    try
                    {


                        var client = _clientFactory.CreateClient();

                        HttpClientHandler clientHandler = new HttpClientHandler();
                        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                        client = new HttpClient(clientHandler);

                        var html = await client.GetStringAsync(rowUrl);

                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        var node = doc.DocumentNode.SelectSingleNode("//body");

                        newHash = HashHelper.ComputeHash(HtmlPageHelper.ProcessHtml(node?.InnerHtml));

                        UpdateRowStatus(dt_processed, rowUrl, newHash, "");

                    }
                    catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException)
                    {
                        UpdateRowStatus(dt_processed, rowUrl, newHash, "INVALID_HOST");
                    }
                    catch
                    {
                        UpdateRowStatus(dt_processed, rowUrl, newHash, "REQUEST_FAILED");
                    }

                }

                DataTable dt_latest = FileHelper.LoadCsv(csvPath);

                MergeTables(dt_latest, dt_processed);

                SaveCsv(dt_latest, csvPath);

                return new APIResponse()
                {
                    Status = true,
                    Message = "Success",
                };

            }

            catch (Exception ex)
            {
                return new APIResponse()
                {
                    Status = false,
                    Message = "Technical Error"
                };
            }
        }

        public void MergeTables(DataTable dtLatest, DataTable dtProcessed)
        {
            foreach (DataRow rowProc in dtProcessed.Rows)
            {
                string url = rowProc["URL"].ToString().Trim();

                var rowLatest = dtLatest.AsEnumerable()
                                        .FirstOrDefault(r =>
                                            r["URL"].ToString().Trim()
                                            .Equals(url, StringComparison.OrdinalIgnoreCase));

                if (rowLatest != null)
                {
                    foreach (DataColumn col in dtProcessed.Columns)
                    {
                        if (dtLatest.Columns.Contains(col.ColumnName))
                        {
                            rowLatest[col.ColumnName] = rowProc[col.ColumnName];
                        }
                    }
                }
            }
        }
        public static void UpdateRowStatus(DataTable dt, string url, string newHash, string status)
        {
            foreach (DataRow r in dt.Rows)
            {
                string rowUrl = (r["URL"]?.ToString() ?? "").Trim();

                if (string.Equals(rowUrl, url, StringComparison.OrdinalIgnoreCase))
                {
                    string oldHash = (r["StoredHash"]?.ToString() ?? "").Trim();

                    if (status == "INVALID_HOST")
                    {
                        r["StoredHash"] = newHash;
                        r["LastUpdated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        r["Status"] = status;
                    }
                    else if (status == "REQUEST_FAILED")
                    {
                        r["StoredHash"] = newHash;
                        r["LastUpdated"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        r["Status"] = status;
                    }
                    else if (string.IsNullOrEmpty(oldHash))
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
        public void SaveCsv(DataTable dt, string path)
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
        public void SaveCsv(List<UrlRecord> list, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(list);
            }
        }

    }
}
