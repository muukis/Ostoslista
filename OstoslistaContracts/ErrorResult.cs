using System.Net;

namespace OstoslistaContracts
{
    public class ErrorResult
    {
        public HttpStatusCode Code { get; set; }
        public ErrorClassification Classification { get; set; }
        public string Message { get; set; }
    }
}
