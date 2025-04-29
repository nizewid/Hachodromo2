using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Hachodromo.API.Helpers
{
    public class FileStorage : IFileStorage
    {
        private readonly string _connectionString;
        public FileStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage")!;
        }

        public async Task RemoveFileAsync(string path, string containerName)
        {
            var client = new BlobContainerClient(_connectionString, containerName);
            await client.CreateIfNotExistsAsync();
            var fileName = Path.GetFileName(path);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> SaveFileAsync(byte[] content, string extension, string containerName)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("La cadena de conexión no puede ser nula o vacía.");

            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("El nombre del contenedor no puede ser nulo o vacío.", nameof(containerName));

            var client = new BlobContainerClient(_connectionString, containerName.ToLower());
            await client.CreateIfNotExistsAsync();
            await client.SetAccessPolicyAsync(PublicAccessType.Blob);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);

            await using var ms = new MemoryStream(content);
            await blob.UploadAsync(ms);

            return blob.Uri.ToString();
        }
    }
}

