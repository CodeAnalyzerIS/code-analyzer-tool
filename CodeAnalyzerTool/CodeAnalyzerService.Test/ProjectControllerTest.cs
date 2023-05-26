using AutoFixture;
using CodeAnalyzerService.Backend.Controllers;
using CodeAnalyzerService.Backend.DTOs.Request;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

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
        //Arrange
        var autoFixture = new Fixture();
        // var ruleViolationDto = autoFixture.Build<RuleViolationDto>().With(rv => rv.Rule, "Info").CreateMany(3);
        // var projectAnalysisDto = autoFixture.Build<ProjectAnalysisDto>().With(pa => pa.RuleViolations, new List<RuleViolationDto>(){ruleViolationDto}).Create();
        var projectRequest = autoFixture.Create<ProjectAnalysisRequest>();
        await using var context = Fixture.CreateContext();
        var controller = new ProjectController(context);
        
        //Act
        var ar = await controller.PutProject(projectRequest);

        //Assert
        var result = ar.Result as CreatedAtActionResult;
        result.Should().NotBeNull();
        Assert.Single(context.Projects);
        Assert.Single(context.Analyses);
        
        await context.Database.EnsureDeletedAsync();
    }
}