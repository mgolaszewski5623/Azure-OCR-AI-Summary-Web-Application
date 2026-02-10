using Azure.Storage.Blobs;

namespace Project.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            string? connectionString = config["BlobStorage:ConnectionString"];
            string? containerName = config["BlobStorage:ContainerName"];

            _container = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            BlobClient blobClient = _container.GetBlobClient(fileName);

            using Stream stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
