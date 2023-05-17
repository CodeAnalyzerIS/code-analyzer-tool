using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public sealed class CodeAnalyzerServiceDbContext : DbContext
{
    public CodeAnalyzerServiceDbContext(DbContextOptions<CodeAnalyzerServiceDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<RuleViolation> RuleViolations { get; set; } = null!;
    public DbSet<Rule> Rules { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Analysis> Analyses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureRuleViolations(modelBuilder);
        ConfigureRule(modelBuilder);
        ConfigureProjectAnalysis(modelBuilder);
        ConfigureAnalysis(modelBuilder);
    }

    private void ConfigureRuleViolations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RuleViolation>()
            .OwnsOne(ar => ar.Location);

        modelBuilder.Entity<RuleViolation>()
            .HasOne(ar => ar.Rule)
            .WithMany(r => r.RuleViolations)
            .HasForeignKey("FK_RuleViolation_Rule");
    }

    private void ConfigureRule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rule>()
            .HasIndex(r => r.RuleName)
            .IsUnique();
    }

    private void ConfigureProjectAnalysis(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany(pa => pa.Analyses)
            .WithOne(rv => rv.Project)
            .HasForeignKey("FK_ProjectAnalysis_Analysis");

        modelBuilder.Entity<Project>()
            .HasIndex(pa => pa.ProjectName)
            .IsUnique();
    }

    private void ConfigureAnalysis(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analysis>()
            .HasMany(a => a.RuleViolations)
            .WithOne(rv => rv.Analysis)
            .HasForeignKey("FK_Analysis_RuleViolation");
    }
}