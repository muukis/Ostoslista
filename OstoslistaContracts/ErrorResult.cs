namespace OstoslistaContracts
{
    public class ErrorResult
    {
        public int Code { get; set; }
        public ErrorClassification Classification { get; set; }
        public string Message { get; set; }
    }
}
