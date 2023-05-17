using System.Diagnostics;
using AutoFixture;
using CodeAnalyzerService.Backend.Controllers;
using CodeAnalyzerService.Backend.Dtos;

namespace CodeAnalyzerService.Test;

public class ProjectControllerTest : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; }

    public ProjectControllerTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public async Task PutProjectReturnsCreatedAt()
    {
        var autoFixture = new Fixture();
        // var ruleViolationDto = autoFixture.Build<RuleViolationDto>().With(rv => rv.Rule, "Info").CreateMany(3);
        // var projectAnalysisDto = autoFixture.Build<ProjectAnalysisDto>().With(pa => pa.RuleViolations, new List<RuleViolationDto>(){ruleViolationDto}).Create();
        var projectAnalysisDto = autoFixture.Create<ProjectAnalysisDto>();
        using var context = Fixture.CreateContext();
        var controller = new ProjectController(context);
        var ar = await controller.PutProject(projectAnalysisDto);
        await context.Database.EnsureDeletedAsync();
        Debugger.Break();
    }
}