namespace ShareWithMe.Services;

public interface IFileStorageService {

    // 
    /// Saves a file to the storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to save the file to.</param>
    /// <param name="fileStream">The stream of the file to save.</param>
    /// <returns>The URL of the saved file.</returns>
    Task<string>  SaveAsync(string blobName, Stream fileStream, CancellationToken cancellationToken = default);

    // Open a file from the storage.
    /// <summary>
    /// Opens a file from the storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to open the file from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stream of the opened file.</returns>
    Task<Stream> OpenReadAsync(string blobName, CancellationToken cancellationToken = default)?;



}