namespace ShareWithMe.Services;

public interface IFileStorageService {

    // 
    /// Generates a SAS upload URL for a given blob name.
    /// </summary>
    /// <param name="blobName">The name of the blob to generate the SAS upload URL for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The SAS upload URL.</returns>
    /// <remarks>
    /// Allows Upload from Client directly to Azure Blob Storage.
    /// Reduces connectivity and bandwidth requirements for large file load on server. 

    Uri GenerateSasUploadUrl(string blobName, CancellationToken cancellationToken = default);

    Uri GenerateSasDownloadUrl(string blobName, string originalFileName);



    // Open a file from the storage.
    /// <summary>
    /// Opens a file from the storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to open the file from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stream of the opened file.</returns>
    Task<Stream> OpenReadAsync(string blobName, CancellationToken cancellationToken = default);



}