using System.Collections.Generic;
using System.Threading.Tasks;
using MoneyManager.Models;

namespace MoneyManager.Repository
{
    public interface IMoneyUsageRepository
    {
        public Task<IEnumerable<MoneyUsage>> SaveMoneyUsage(List<MoneyUsage> moneyUsages);
    }
}