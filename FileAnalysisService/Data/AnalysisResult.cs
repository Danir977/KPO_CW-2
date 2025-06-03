namespace file_analysis_service.Data;

public class AnalysisResult
{
    public int Id { get; set; }
    public string FileId { get; set; }
    public int WordCount { get; set; }
    public int CharacterCount { get; set; }
    public int ParagraphCount { get; set; }
    public DateTime AnalyzedAt { get; set; }
}