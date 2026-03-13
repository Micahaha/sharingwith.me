using Microsoft.AspNetCore.Mvc;
using ShareWithMe.Data;
using ShareWithMe.Services;
using ShareWithMe.Models;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

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


    // return 201 after successful upload
   
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null) {
            return BadRequest(new { Message = "No file uploaded" });
        }

        if (file.Length == 0) {
            return BadRequest(new { Message = "File is empty" });
        }

        if (file.ContentType == null) {
            return BadRequest(new { Message = "File content type is null" });
        }

        var fileStream = file.OpenReadStream();
        var blobName = Guid.NewGuid().ToString();


        
        try {
            // Save file to blob storage
        await _fileStorageService.SaveAsync(blobName, fileStream);

            // save file to database (MySQL database)
        var fileItem = new FileItem
        {
            OriginalFileName = file.FileName,
            BlobName = blobName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            IsPublic = true,
        };
        
        await _dbContext.Files.AddAsync(fileItem);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFile), new { shareCode = fileItem.shareCode }, new { url = $"/api/files/{fileItem.shareCode}" });
        
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

        // shared_file is the shared blob name designed to match my blobfile in az stroage. 
        var shared_file_BlobName = shared_file_record.BlobName;
        


        var fileStream = await _fileStorageService.OpenReadAsync(shared_file_BlobName);
        if (fileStream == null) return NotFound();

        var fileContent = new FileStreamResult(fileStream, shared_file_record.ContentType);

 
        return fileContent;

        // test the file stream result
    }
}