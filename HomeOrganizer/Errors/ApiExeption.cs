namespace HomeOrganizer.Errors
{
    public class ApiExeption : ApiResponse
    {
        public ApiExeption(int statusCode, string message = null, string details 
        = null) : base(statusCode, message)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}