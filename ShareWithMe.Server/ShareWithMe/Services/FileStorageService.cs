namespace ShareWithMe.Services;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using ShareWithMe.Services;

public class FileStorageService : IFileStorageService {
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly string blobContainerConnectionString;

    public FileStorageService(IConfiguration configuration) {

        _configuration = configuration;
        blobContainerConnectionString = _configuration.GetConnectionString("BlobStorageConnectionString");
        _containerName = _configuration.GetSection("FileStorage:ContainerName").Value;

    }
    public async Task<string> SaveAsync(string blobName, Stream fileStream, CancellationToken cancellationToken = default) {
        var blobServiceClient = new BlobServiceClient(blobContainerConnectionString);

        // stream file from fileStream to blob container.
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        fileStream.Seek(0, SeekOrigin.Begin);

        await blobContainerClient.UploadBlobAsync(blobName, fileStream);
        return blobName;
    }


    // Open a file from the storage.
    public async Task<Stream> OpenReadAsync(string blobName, CancellationToken cancellationToken = default) {
    var blobServiceClient = new BlobServiceClient(blobContainerConnectionString);
    var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
    var blobClient = blobContainerClient.GetBlobClient(blobName);

    return await blobClient.OpenReadAsync(new BlobOpenReadOptions(allowModifications: false), cancellationToken);
    }
}