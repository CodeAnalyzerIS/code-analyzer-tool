using CAS_Backend.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CAS_Backend.EF;

public class CasContext : DbContext
{
    public CasContext(DbContextOptions<CasContext> options) : base(options)
    {
    }

    public DbSet<AnalysisResult> AnalysisResults { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
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