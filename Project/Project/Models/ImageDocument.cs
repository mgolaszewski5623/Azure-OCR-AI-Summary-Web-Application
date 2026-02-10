using System.Text.Json.Serialization;

namespace Project.Models
{
    public class ImageDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; } = default!;

        public string FileName { get; set; } = default!;
        public string ImageUrl { get; set; }
        public string ExtractedText { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
