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
    }
}