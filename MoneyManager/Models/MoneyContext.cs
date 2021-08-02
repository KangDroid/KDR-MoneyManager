using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace MoneyManager.Models
{
    public class MoneyContext: DbContext
    {
        [ExcludeFromCodeCoverage]
        public MoneyContext(DbContextOptions<DbContext> options) : base(options)
        {
        }
        
        public DbSet<MoneyUsage> MoneyUsages { get; set; }
    }
}