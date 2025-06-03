using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace file_storage_service.Controllers;

public class FileController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");
        var id = Guid.NewGuid().ToString();
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var filePath = Path.Combine(folder, id);
        using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);
        return Ok(new { FileId = id });
    }

    [HttpGet("{id}")]
    public IActionResult Download(string id)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", id);
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        var content = System.IO.File.OpenRead(filePath);
        return File(content, "application/octet-stream", id);
    }
}