namespace ShareWithMe.Dtos;

// just a dto to pass the file information to the controller.

public class RegisterFileRequest
{
    public string BlobName { get; set; }

    public string OriginalFileName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
}