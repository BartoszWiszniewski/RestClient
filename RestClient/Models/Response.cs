namespace RestClient.Models
{
    using System.Net;

    public class Response<T>
    {
        public T Content { get; set; }

        public string ErrorMessage { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool Success { get; set; }
    }
}