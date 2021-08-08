using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoneyManager.Communication;
using MoneyManager.Models;

namespace MoneyManager.Service
{
    public interface IMoneyUsageService
    {
        public Task<Result<List<MoneyUsage>>> GetMoneyUsageFromCsv(IFormFile csvFile);
        public Task<Result<List<MoneyUsage>>> SaveMoneyUsage(List<MoneyUsage> moneyUsages);
    }
}