using CodeAnalyzerService.Backend.DAL;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.Managers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddControllers();
builder.Services.AddDbContext<CodeAnalyzerServiceDbContext>(opt =>
    opt.UseSqlite(connectionString!));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRuleViolationRepository, RuleViolationRepository>();
builder.Services.AddScoped<IRuleRepository, RuleRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();
builder.Services.AddScoped<IProjectAnalysisManager, ProjectAnalysisManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
