
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Data;

public class AnalysisDbContext : DbContext
{
    public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) {}
    public DbSet<AnalysisResult> Results => Set<AnalysisResult>();
}
