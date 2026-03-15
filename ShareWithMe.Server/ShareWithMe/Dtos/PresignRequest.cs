namespace ShareWithMe.Dtos;

// just a dto to pass the file information to the controller.

public class PresignRequest
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
}