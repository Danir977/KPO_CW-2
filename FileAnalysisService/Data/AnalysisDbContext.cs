using Microsoft.EntityFrameworkCore;

namespace file_analysis_service.Data;

public class AnalysisDbContext : DbContext {
    public DbSet<AnalysisResult> AnalysisResults { get; set; }
    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) {}
}
