using Microsoft.Azure.Cosmos;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Project.Services
{
    public class ComputerVisionService
    {
        private readonly string _endpoint;
        private readonly string _key;
        private readonly HttpClient _httpClient;

        public ComputerVisionService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureComputerVision:Endpoint"] + "/vision/v3.2/ocr?language=unk&detectOrientation=true";
            _key = configuration["AzureComputerVision:Key"];
            _httpClient = new HttpClient();
        }

        public async Task<string> ExtractTextFromImageAsync(byte[] imageBytes)
        {
            using ByteArrayContent content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);

            HttpResponseMessage response = await _httpClient.PostAsync(_endpoint, content);
            if (!response.IsSuccessStatusCode) 
                return "Failed to process image.";

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);

            string extractedText = "";
            /*
            regions
            └ lines
              └ words
                └ text
            */
            foreach (JsonElement region in doc.RootElement.GetProperty("regions").EnumerateArray())
            {
                foreach (JsonElement line in region.GetProperty("lines").EnumerateArray())
                {
                    foreach (JsonElement word in line.GetProperty("words").EnumerateArray())
                    {
                        extractedText += word.GetProperty("text").GetString() + " ";
                    }
                    extractedText += "\n";
                }
            }

            return extractedText.Trim();
        }
    }
}