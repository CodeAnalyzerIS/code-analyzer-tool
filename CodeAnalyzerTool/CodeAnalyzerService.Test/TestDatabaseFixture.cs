using CodeAnalyzerService.Backend.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Test;

public class TestDatabaseFixture
{
    private const string CONNECTION_STRING = @"Data Source=CodeAnalyzerServiceTest.db";

    //alternatief semaphorslim -> lock misschien niet nodig
    private static readonly object Lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (Lock)
        {
            if (_databaseInitialized) return;
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            _databaseInitialized = true;
        }
    }

    public CodeAnalyzerServiceDbContext CreateContext()
        => new(new DbContextOptionsBuilder<CodeAnalyzerServiceDbContext>()
                .UseSqlite(CONNECTION_STRING)
                .Options);
}