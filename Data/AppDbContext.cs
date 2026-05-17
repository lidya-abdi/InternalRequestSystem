using Microsoft.EntityFrameworkCore;    
using InternalRequestSystem.Models;


namespace InternalRequestSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Approval> Approvals { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Approval>()
                .HasOne(a => a.Request)
                .WithMany(r => r.Approvals)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Approval>()
                .HasOne(a => a.ApprovedByUser)
                .WithMany(u => u.Approvals)
                .HasForeignKey(a => a.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
