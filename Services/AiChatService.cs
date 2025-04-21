using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsignmentWebsite.Services
{
    public class AiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "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"; // Thay bằng API key thật

        public AiChatService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> AskChatGptAsync(string userPrompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = userPrompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"❌ Error: {response.StatusCode} - {responseString}";
            }

            using (var jsonDoc = JsonDocument.Parse(responseString))
            {
                return jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
        }
    }
}
