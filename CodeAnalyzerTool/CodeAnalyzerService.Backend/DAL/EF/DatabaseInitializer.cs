using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public static class DatabaseInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        //Creating scope to get an instance of context to create database and then dispose of instance
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CodeAnalyzerServiceDbContext>();
        context.Database.EnsureCreated();
    }
}