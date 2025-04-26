
using FridgeProject.Data.Models.GeminiModels;
using FridgeProject.Data.Models.GeminiModels.ContentResponse;
using FridgeProject.DTOs;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;


namespace FridgeProject.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "..";
        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetGeminiResponseAsync(string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
            var request = new ContentRequest
            {
                contents = new[]
                {
                    new Data.Models.GeminiModels.Content
                    {
                        parts = new[]
                        {
                            new Data.Models.GeminiModels.Part
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {response.StatusCode}, Details: {errorMessage}");
            }

            var jsonResponce = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ContentResponse>(jsonResponce);

            return result?.Candidates.FirstOrDefault()?.Content?.Parts[0].Text ?? "No response from Gemini";

        }
    }

}
//AIzaSyDRIFwXAJksNx-WhejjAoppi7YTvLRXA_A