using Microsoft.EntityFrameworkCore;
using ShareWithMe.Models;
namespace ShareWithMe.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }



    public DbSet<FileItem> Files => Set<FileItem>();
}

