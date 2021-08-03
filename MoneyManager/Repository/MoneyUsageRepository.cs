using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyManager.Models;

namespace MoneyManager.Repository
{
    public class MoneyUsageRepository: IMoneyUsageRepository
    {
        private readonly MoneyContext _moneyContext;
        
        public MoneyUsageRepository(MoneyContext moneyContext)
        {
            _moneyContext = moneyContext;
        }
        
        // Add Money to DB
        public async Task<IEnumerable<MoneyUsage>> SaveMoneyUsage(List<MoneyUsage> moneyUsages)
        {
            await _moneyContext.AddRangeAsync(moneyUsages);
            await _moneyContext.SaveChangesAsync();

            return moneyUsages;
        }
    }
}