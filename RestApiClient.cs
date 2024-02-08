using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using pipelineAuditor.Models;

namespace pipelineAuditor
{
    class RestApiClient
        // A class whose job is to construct and make an API call for a specific pipeline run to get relevant information regarding that run
    {
        // FIELDS
        private string _PersonalAccessToken;

        // PROPERTIES
        public string RawResponseBody { get; private set; }
        public string IssuesString { get; private set; }

        // CONSTRUCTOR
        public RestApiClient(string PersonalAccessToken)
        {
            this._PersonalAccessToken = PersonalAccessToken;
        }

        public async Task<string> GetResponse(string apiCall)
        {
            using (var client = new HttpClient())
            {
                // Set the base URL for HTML response body
                client.BaseAddress = new Uri(apiCall);
                // Clear and set the Accept header
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Add the Authorization header with the PAT
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{this._PersonalAccessToken}")));

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    RawResponseBody = responseBody;
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    return null;
                }
            }
        }
    }
}

