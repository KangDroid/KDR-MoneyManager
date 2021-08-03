using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MoneyManager.Models;
using MoneyManager.Repository;
using Xunit;
using Xunit.Sdk;

namespace MoneyManagerTest.Repository
{
    public class MoneyUsageRepositoryTest
    {
        private readonly IMoneyUsageRepository _moneyRepository;
        private readonly MoneyContext _moneyContext;
        
        public MoneyUsageRepositoryTest()
        {
            var dbOptions = new DbContextOptionsBuilder<MoneyContext>().UseInMemoryDatabase("test_db").Options;
            _moneyContext = new MoneyContext(dbOptions);
            _moneyRepository = new MoneyUsageRepository(_moneyContext);
        }

        [Fact(DisplayName = "SaveMoneyUsage saves emptyList well")]
        public async void Is_SaveMoneyUsage_Saves_emptyList_Well()
        {
            // Do
            var resultList = await _moneyRepository.SaveMoneyUsage(new List<MoneyUsage>());
            
            // Check
            Assert.Empty(resultList);
            Assert.Empty(_moneyContext.MoneyUsages.ToList());
        }
    }
}