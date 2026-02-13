using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessTendors.Application.Common.Models
{
    public class UrlRecord
    {
        public string URL { get; set; }
        public string StoredHash { get; set; }
        public string LastUpdated { get; set; }
        public string Status { get; set; }
    }
}
