using Microsoft.Azure.Cosmos;
using Project.Models;

namespace Project.Services
{
    public class CosmosDbService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public CosmosDbService(CosmosClient client, IConfiguration config)
        {
            _container = client.GetContainer(
                config["CosmosDb:DatabaseName"],
                config["CosmosDb:ContainerName"]);
        }

        public async Task AddDocumentAsync(ImageDocument imageDocument)
        {
            var doc = new
            {
                id = string.IsNullOrWhiteSpace(imageDocument.Id) ? Guid.NewGuid().ToString() : imageDocument.Id,
                sessionId = imageDocument.SessionId,
                fileName = imageDocument.FileName,
                ImageUrl = imageDocument.ImageUrl,
                extractedText = imageDocument.ExtractedText,
                Timestamp = DateTime.UtcNow
            };
            await _container.CreateItemAsync(doc, new PartitionKey(imageDocument.SessionId));
        }

        public async Task<List<ImageDocument>> GetAllDocumentsAsync()
        {
            FeedIterator<ImageDocument> query = _container.GetItemQueryIterator<ImageDocument>("SELECT * FROM c");
            List<ImageDocument> results = new List<ImageDocument>();

            while (query.HasMoreResults)
            {
                FeedResponse<ImageDocument> response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<ImageDocument?> GetDocumentByIdAsync(string id)
        {
            List<ImageDocument> records = await GetAllDocumentsAsync();
            return records.FirstOrDefault(r => r.Id == id);
        }
    }
}