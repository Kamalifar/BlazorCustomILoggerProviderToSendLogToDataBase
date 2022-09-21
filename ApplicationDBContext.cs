using Microsoft.EntityFrameworkCore;
using SendBlazorLoggerToDataBase.Entities;

namespace SendBlazorLoggerToDataBase
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<DBLog> DbLogs{ get; set; }
    }
}