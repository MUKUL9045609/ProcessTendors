
namespace ProcessTendors.Application.Common.Models
{
    public class APIResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string Token { get; set; }
        public int ExpiryInSeconds { get; set; }
    }
}
