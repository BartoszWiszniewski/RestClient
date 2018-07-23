namespace RestClient.Services.Interfaces
{
    using System.Collections.Specialized;

    using RestClient.Models;

    public interface IRestClientService
    {
        Response<string> Delete(string url, NameValueCollection headers = null);

        Response<T> Delete<T>(string url, NameValueCollection headers = null);

        Response<string> Get(string url, NameValueCollection headers = null);

        Response<T> Get<T>(string url, NameValueCollection headers = null);

        Response<string> Post(string url, string payload, NameValueCollection headers = null);

        Response<TResponse> Post<TRequest, TResponse>(string url, TRequest data, NameValueCollection headers = null);

        Response<string> Put(string url, string payload, NameValueCollection headers = null);

        Response<TResponse> Put<TRequest, TResponse>(string url, TRequest data, NameValueCollection headers = null);
    }
}