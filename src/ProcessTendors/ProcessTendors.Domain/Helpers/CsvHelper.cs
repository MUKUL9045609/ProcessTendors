using System.Data;

namespace ProcessTendors.Domain.Helpers
{
    public static class CsvHelper
    {
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
    }
}
