using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            // Remove any duplicated contents.
            foreach (var eachMoneyUsage in moneyUsages)
            {
                var isExists = _moneyContext.MoneyUsages.SingleOrDefault(a => a.GradId == eachMoneyUsage.GradId);
                if (isExists != null)
                {
                    _moneyContext.Remove(isExists);
                }
            }
            await _moneyContext.SaveChangesAsync();
            
            // Add them
            await _moneyContext.AddRangeAsync(moneyUsages);
            await _moneyContext.SaveChangesAsync();

            return moneyUsages;
        }

        public async Task<IEnumerable<MoneyUsage>> GetMoneyUsage()
        {
            return await _moneyContext.MoneyUsages.ToListAsync();
        }
    }
}