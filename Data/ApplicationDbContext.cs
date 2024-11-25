using Microsoft.EntityFrameworkCore;
using SimpleDriveProject.Models;

namespace SimpleDriveProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<BlobStorage> BlobStorages { get; set; }
    }
}
