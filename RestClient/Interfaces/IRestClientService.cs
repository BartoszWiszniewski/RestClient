namespace RestClient.Services.Interfaces
{
    using RestClient.Models;

    public interface IRestClientService
    {
        Response<string> Delete(string url);

        Response<T> Delete<T>(string url);

        Response<string> Get(string url);

        Response<T> Get<T>(string url);

        Response<string> Post(string url, string payload);

        Response<TResponse> Post<TRequest, TResponse>(string url, TRequest data);

        Response<string> Put(string url, string payload);

        Response<TResponse> Put<TRequest, TResponse>(string url, TRequest data);
    }
}