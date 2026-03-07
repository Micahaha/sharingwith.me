namespace ShareWithMe.Services;


using Microsoft.Extensions.Configuration;
using ShareWithMe.Services;

public class FileStorageService : IFileStorageService {
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly string _containerName;

    public FileStorageService(IConfiguration configuration) {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _containerName = _configuration.GetSection("FileStorage:ContainerName").Value;
    }
    public Task<string> SaveAsync(string blobName, Stream fileStream, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    public Task<Stream> OpenReadAsync(string blobName, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}