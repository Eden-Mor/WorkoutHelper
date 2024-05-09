using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using WorkoutHelper.Data;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace WorkoutHelper.Services
{
    public enum HttpMethod
    {
        Get = 0,
        Post = 1,
        Put = 2,
        Delete = 3
    }
    public static class RestService
    {

        static HttpClient _httpClient;
        static JsonSerializerOptions _serializerOptions;

        static RestService()
        {
            _httpClient = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public static void SetBearer(string token)
        {
            SessionService.Token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<T?> CallAPIMethod<T>(string method, HttpMethod type = HttpMethod.Get, string data = "")
        {
            T? item = default;

            Uri uri = new(Path.Combine(Constants.API_BASE_PATH, method));
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = type switch
                {
                    HttpMethod.Get => await _httpClient.GetAsync(uri),
                    HttpMethod.Post => await _httpClient.PostAsync(uri, content),
                    HttpMethod.Put => await _httpClient.PutAsync(uri, content),
                    HttpMethod.Delete => await _httpClient.DeleteAsync(uri),
                    _ => await _httpClient.GetAsync(uri),
                };

                string responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && typeof(T) != typeof(string))
                    item = JsonSerializer.Deserialize<T>(responseData, _serializerOptions);
                else if (response.IsSuccessStatusCode && typeof(T) == typeof(string))
                    item = (T)Convert.ChangeType(responseData, typeof(T));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return item;
        }
    }
}
