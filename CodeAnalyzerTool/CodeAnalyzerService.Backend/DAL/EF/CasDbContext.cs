using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class CasDbContext : DbContext
{
    public CasDbContext()
    {
    }
    
    public CasDbContext(DbContextOptions<CasDbContext> options) : base(options)
    {
    }

    public DbSet<AnalysisResult> AnalysisResults { get; set; } = null!;
    public DbSet<Rule> Rules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureAnalysisResults(modelBuilder);
        ConfigureRule(modelBuilder);
    }

    private void ConfigureAnalysisResults(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisResult>()
            .OwnsOne(ar => ar.Location);

        modelBuilder.Entity<AnalysisResult>()
            .HasOne(ar => ar.Rule)
            .WithMany(r => r.AnalysisResults)
            .HasForeignKey("FK_AnalysisResult_Rule");
    }

    private void ConfigureRule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rule>()
            .HasIndex(r => r.RuleName)
            .IsUnique();
    }
}