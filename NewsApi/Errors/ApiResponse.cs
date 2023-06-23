namespace News.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        private static string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request was received by the server.",
                401 => "The request requires authentication.",
                404 => "The requested resource could not be found.",
                500 => "An internal server error occurred.",
                _ => null,
            };
        }
    }
}
