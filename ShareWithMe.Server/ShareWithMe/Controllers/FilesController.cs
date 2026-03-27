using Microsoft.AspNetCore.Mvc;
using ShareWithMe.Data;
using ShareWithMe.Services;
using ShareWithMe.Models;
using ShareWithMe.Dtos;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ShareWithMe.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly AppDbContext _dbContext;


    public FilesController(IFileStorageService fileStorageService, AppDbContext dbContext)
    {
        _fileStorageService = fileStorageService;
        _dbContext = dbContext;
    }




    [HttpPost("presign")]
    public IActionResult PresignUpload([FromBody] PresignRequest request)
    {
        var blobName = Guid.NewGuid().ToString();
        var sasUri = _fileStorageService.GenerateSasUploadUrl(blobName);
return Ok(new { sasUrl = sasUri.ToString(), blobName});
    }


    // return 201 after successful upload
   
    [HttpPost]
    public async Task<IActionResult> RegisterFile([FromBody] RegisterFileRequest request)
    {
        try {
            // save file to database (MySQL database)
        var fileItem = new FileItem
        {
            OriginalFileName = request.OriginalFileName,
            BlobName = request.BlobName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            IsPublic = true,
            ExpiresAt = DateTime.UtcNow.AddHours(6),
        };
        
        await _dbContext.Files.AddAsync(fileItem);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFile), new { shareCode = fileItem.shareCode }, new RegisterFileResponse
        {
            Url = $"/api/files/{fileItem.shareCode}",
            ExpiresAt = fileItem.ExpiresAt
        });
        
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An internal server error occurred", Detail = ex.Message });
        }

    }


    // Get a file by its ID. This is the download link.

    [HttpGet("{shareCode}")]
    public async Task<IActionResult> GetFile(string? shareCode)
    {
        if (shareCode == null) return BadRequest(new { Message = "Share code is required" });

        // query entire row from the database
        var shared_file_record = await _dbContext.Files.Where(f => f.shareCode == shareCode).FirstOrDefaultAsync();
        if (shared_file_record == null) return NotFound();


        // Check if file expired

        if (shared_file_record.ExpiresAt.HasValue && shared_file_record.ExpiresAt.Value < DateTime.UtcNow)
            return NotFound(new { Message = $"This file has expired and is no longer available. Expired at: {shared_file_record.ExpiresAt.Value}" });

        var sasUri = _fileStorageService.GenerateSasDownloadUrl(shared_file_record.BlobName, shared_file_record.OriginalFileName);
        return Ok(new { sasUrl = sasUri.ToString(), originalFileName = shared_file_record.OriginalFileName, expiresAt = shared_file_record.ExpiresAt });
    }
}