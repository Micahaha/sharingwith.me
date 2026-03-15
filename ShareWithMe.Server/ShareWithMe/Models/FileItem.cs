using Microsoft.EntityFrameworkCore;
namespace ShareWithMe.Models;



// Represents a file in the database.
// Model class for the FileItem table.


[Index(nameof(shareCode), IsUnique = true)]
public class FileItem
{
    public int Id { get; set; }
    public string shareCode { get; set; } = Random.Shared.Next(0, 100_000_000).ToString("D8");
    public string OriginalFileName { get; set; }
    public string BlobName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}