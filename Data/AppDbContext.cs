using Microsoft.EntityFrameworkCore;    
using InternalRequestSystem.Models;


namespace InternalRequestSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Request> Requests { get; set; }
    }
}
