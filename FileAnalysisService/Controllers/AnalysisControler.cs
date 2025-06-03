using file_analysis_service.Data;
using Microsoft.AspNetCore.Mvc;

namespace file_analysis_service.Controllers;

public class FileAnalysisRequest { public string FileId { get; set; } }

[ApiController]
[Route("api/analysis")]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisDbContext _db;
    private readonly HttpClient _client;
    public AnalysisController(AnalysisDbContext db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _client = httpFactory.CreateClient("fileStorage");
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] FileAnalysisRequest request)
    {
        // Получаем файл из сервиса хранения по ID
        var response = await _client.GetAsync($"/api/files/{request.FileId}");
        if (!response.IsSuccessStatusCode)
            return NotFound("File not found");
        var content = await response.Content.ReadAsStringAsync();

        // Подсчет статистики
        int charCount = content.Length;
        var words = content.Split(new char[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;
        var paragraphs = content.Split(
            new string[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries);
        int paragraphCount = paragraphs.Length;

        // Сохраняем результат в БД
        var result = new AnalysisResult {
            FileId = request.FileId,
            WordCount = wordCount,
            CharacterCount = charCount,
            ParagraphCount = paragraphCount,
            AnalyzedAt = DateTime.UtcNow
        };
        _db.AnalysisResults.Add(result);
        await _db.SaveChangesAsync();

        return Ok(result);
    }

    [HttpGet("results/{id}")]
    public async Task<IActionResult> GetResult(int id)
    {
        var result = await _db.AnalysisResults.FindAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
