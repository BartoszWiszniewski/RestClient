namespace RestClient.Services
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json;

    using RestClient.Models;
    using RestClient.Services.Interfaces;

    public class RestClientService : IRestClientService
    {
        public Response<string> Delete(string url, NameValueCollection headers = null)
        {
            var httpWebRequest = this.BuildRequest(url, "DELETE", headers);
            return this.Request(httpWebRequest);
        }

        public Response<T> Delete<T>(string url, NameValueCollection headers = null)
        {
            var response = this.Delete(url, headers);
            return this.MapResponse<T>(response);
        }

        public Response<string> Get(string url, NameValueCollection headers = null)
        {
            var httpWebRequest = this.BuildRequest(url, "GET", headers);
            return this.Request(httpWebRequest);
        }

        public Response<T> Get<T>(string url, NameValueCollection headers = null)
        {
            var response = this.Get(url, headers);
            return this.MapResponse<T>(response);
        }

        public Response<TResponse> Post<TRequest, TResponse>(string url, TRequest data, NameValueCollection headers = null)
        {
            var payload = JsonConvert.SerializeObject(data);
            var response = this.Post(url, payload, headers);
            return this.MapResponse<TResponse>(response);
        }

        public Response<string> Post(string url, string payload, NameValueCollection headers = null)
        {
            var httpWebRequest = this.BuildRequest(url, "POST", "application/json; charset=UTF-8", "application/json", headers);
            return this.Request(httpWebRequest, payload);
        }

        public Response<TResponse> Put<TRequest, TResponse>(string url, TRequest data, NameValueCollection headers = null)
        {
            var payload = JsonConvert.SerializeObject(data);
            var response = this.Put(url, payload, headers);
            return this.MapResponse<TResponse>(response);
        }

        public Response<string> Put(string url, string payload, NameValueCollection headers = null)
        {
            var httpWebRequest = this.BuildRequest(url, "PUT", "application/json; charset=UTF-8", "application/json", headers);
            return this.Request(httpWebRequest, payload);
        }

        private HttpWebRequest BuildRequest(string url, string method, NameValueCollection headers)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (headers != null)
            {
                request.Headers.Add(headers);
            }

            return request;
        }

        private HttpWebRequest BuildRequest(string url, string method, string contentType, string accept, NameValueCollection headers)
        {
            var request = this.BuildRequest(url, method, headers);
            request.ContentType = contentType;
            request.Accept = accept;
            return request;
        }

        private Response<string> GetResponse(string content, bool succeeded, HttpStatusCode httpStatusCode)
        {
            return new Response<string>
            {
                Content = succeeded ? content : null,
                Success = succeeded,
                ErrorMessage = succeeded ? null : content,
                StatusCode = httpStatusCode
            };
        }

        private Response<string> GetResponseOnException(WebException webException)
        {
            var webResponse = webException.Response as HttpWebResponse;
            var content = string.Empty;
            var statusCode = webResponse.StatusCode;

            using (var stream = webException.Response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return this.GetResponse(content, false, statusCode);
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

        private void PostData(HttpWebRequest request, string payload = null)
        {
            if (!string.IsNullOrEmpty(payload))
            {
                var dataBytes = Encoding.UTF8.GetBytes(payload);
                request.ContentLength = dataBytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }
        }

        private Response<string> Request(HttpWebRequest request, string payload = null)
        {
            try
            {
                this.PostData(request, payload);
                return this.SendRequest(request);
            }
            catch (WebException webException)
            {
                return this.GetResponseOnException(webException);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Response<string> SendRequest(HttpWebRequest request)
        {
            var content = string.Empty;
            var statusCode = HttpStatusCode.OK;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        content = reader.ReadToEnd();
                        statusCode = response.StatusCode;
                    }
                }
            }

            return this.GetResponse(content, true, statusCode);
        }
    }
}