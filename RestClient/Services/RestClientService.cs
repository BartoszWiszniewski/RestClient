namespace RestClient.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json;

    using RestClient.Models;
    using RestClient.Services.Interfaces;

    public class RestClientService : IRestClientService
    {
        public Response<string> Delete(string url)
        {
            var httpWebRequest = this.BuildRequest(url, "DELETE");
            return this.Request(httpWebRequest);
        }

        public Response<T> Delete<T>(string url)
        {
            var response = this.Delete(url);
            return this.MapResponse<T>(response);
        }

        public Response<string> Get(string url)
        {
            var httpWebRequest = this.BuildRequest(url, "GET");
            return this.Request(httpWebRequest);
        }

        public Response<T> Get<T>(string url)
        {
            var response = this.Get(url);
            return this.MapResponse<T>(response);
        }

        public Response<TResponse> Post<TRequest, TResponse>(string url, TRequest data)
        {
            var payload = JsonConvert.SerializeObject(data);
            var response = this.Post(url, payload);
            return this.MapResponse<TResponse>(response);
        }

        public Response<string> Post(string url, string payload)
        {
            var httpWebRequest = this.BuildRequest(url, "POST", "application/json; charset=UTF-8", "application/json");
            return this.Request(httpWebRequest, payload);
        }

        public Response<TResponse> Put<TRequest, TResponse>(string url, TRequest data)
        {
            var payload = JsonConvert.SerializeObject(data);
            var response = this.Put(url, payload);
            return this.MapResponse<TResponse>(response);
        }

        public Response<string> Put(string url, string payload)
        {
            var httpWebRequest = this.BuildRequest(url, "PUT", "application/json; charset=UTF-8", "application/json");
            return this.Request(httpWebRequest, payload);
        }

        private HttpWebRequest BuildRequest(string url, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            return request;
        }

        private HttpWebRequest BuildRequest(string url, string method, string contentType, string accept)
        {
            var request = this.BuildRequest(url, method);
            request.ContentType = contentType;
            request.Accept = accept;
            return request;
        }

        private Response<T> MapResponse<T>(Response<string> response)
        {
            var content = JsonConvert.DeserializeObject<T>(response.Content);
            return new Response<T>
            {
                Content = content,
                StatusCode = response.StatusCode,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }

        private Response<string> Request(HttpWebRequest request, string payload = null)
        {
            var content = string.Empty;
            var succeeded = false;
            var statusCode = HttpStatusCode.OK;

            try
            {
                if (!string.IsNullOrEmpty(payload))
                {
                    var postBytes = Encoding.UTF8.GetBytes(payload);
                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postBytes, 0, postBytes.Length);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var dataStream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            content = reader.ReadToEnd();
                            statusCode = response.StatusCode;
                            succeeded = true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var webResponse = ex.Response as HttpWebResponse;
                statusCode = webResponse.StatusCode;
                using (var stream = ex.Response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new Response<string>
            {
                Content = succeeded ? content : null,
                Success = succeeded,
                ErrorMessage = succeeded ? null : content,
                StatusCode = statusCode
            };
        }
    }
}