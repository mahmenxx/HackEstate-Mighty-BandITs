using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HackEstate.Managers
{
    public class FaceComparisonService
    {
        private readonly string apiKey = "n-uANst-d1I8RrgncEeiMpgX4IaDOcxp";
        private readonly string apiSecret = "3vRRxZf6VZgCIr1cnrKRj3iSEOYEDicH";
        private readonly string apiUrl = "https://api-us.faceplusplus.com/facepp/v3/compare";
        private readonly string detectApiUrl = "https://api-us.faceplusplus.com/facepp/v3/detect";
        private readonly HttpClient _httpClient;

        public FaceComparisonService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<JObject> CompareFaces(string imageBase64_1, string imageBase64_2)
        {
            try
            {
                var content = new MultipartFormDataContent();

                var apiKeyContent = new StringContent(apiKey);
                apiKeyContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"api_key\""
                };
                content.Add(apiKeyContent);

                var apiSecretContent = new StringContent(apiSecret);
                apiSecretContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"api_secret\""
                };
                content.Add(apiSecretContent);

                var imageBase64_1Content = new StringContent(imageBase64_1);
                imageBase64_1Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"image_base64_1\""
                };
                content.Add(imageBase64_1Content);

                var imageBase64_2Content = new StringContent(imageBase64_2);
                imageBase64_2Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"image_base64_2\""
                };
                content.Add(imageBase64_2Content);

                // Log the content for debugging
                foreach (var item in content)
                {
                    var contentItem = item as StringContent;
                    if (contentItem != null)
                    {
                        var value = await contentItem.ReadAsStringAsync();
                        Console.WriteLine($"{item.Headers.ContentDisposition.Name}: {value}");
                    }
                }

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error response from server: {response.StatusCode} - {errorResponse}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while comparing faces: {ex.Message}", ex);
            }
        }

        public async Task<int> DetectNumberOfFaces(string imageBase64)
        {
            try
            {
                var content = new MultipartFormDataContent();

                content.Add(new StringContent(apiKey), "\"api_key\"");
                content.Add(new StringContent(apiSecret), "\"api_secret\"");
                content.Add(new StringContent(imageBase64), "\"image_base64\"");

                var response = await _httpClient.PostAsync(detectApiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error response from server: {response.StatusCode} - {errorResponse}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseString);

                // Parse the number of faces detected
                var faces = responseJson["faces"] as JArray;
                if (faces == null)
                    return 0; // No faces detected
                return faces.Count;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while detecting faces: {ex.Message}", ex);
            }
        }
    }
}
