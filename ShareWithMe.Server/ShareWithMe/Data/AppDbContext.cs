using Microsoft.EntityFrameworkCore;

namespace ShareWithMe.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }



    public DbSet<FileItem> Files => Set<FileItem>();
}

