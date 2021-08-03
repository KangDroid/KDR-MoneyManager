using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace MoneyManager.Models
{
    public class MoneyContext: DbContext
    {
        [ExcludeFromCodeCoverage]
        public MoneyContext(DbContextOptions<MoneyContext> options) : base(options)
        {
        }
        
        public DbSet<MoneyUsage> MoneyUsages { get; set; }
    }
}