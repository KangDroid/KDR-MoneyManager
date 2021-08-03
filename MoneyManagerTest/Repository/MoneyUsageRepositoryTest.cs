using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoneyManager.Models;
using MoneyManager.Repository;
using Xunit;

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
            _moneyContext.MoneyUsages.RemoveRange(_moneyContext.MoneyUsages);
            _moneyContext.SaveChanges();
            _moneyRepository = new MoneyUsageRepository(_moneyContext);
        }

        [Fact(DisplayName = "SaveMoneyUsage saves emptyList well.")]
        public async void Is_SaveMoneyUsage_Saves_emptyList_Well()
        {
            // Do
            var resultList = await _moneyRepository.SaveMoneyUsage(new List<MoneyUsage>());
            
            // Check
            Assert.Empty(resultList);
            Assert.Empty(_moneyContext.MoneyUsages.ToList());
        }

        [Theory(DisplayName = "SaveMoneyUsage saves multiple random money list well.")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        public async void Is_SaveMoneyUsage_Saves_Multiple_Entity_Well(int count)
        {
            // Generate Random Data
            var emptyList = new List<MoneyUsage>(count);
            for (int i = 0; i < count; i++)
            {
                emptyList.Add(new MoneyUsage
                {
                    GradId = Guid.NewGuid().ToString(),
                    UserDate = DateTime.Now,
                    CardType = Guid.NewGuid().ToString(),
                    UsedLocation = Guid.NewGuid().ToString(),
                    MoneyUsed = 21600,
                    UsedType = Guid.NewGuid().ToString()
                });
            }
            
            // Do
            var resultList = (await _moneyRepository.SaveMoneyUsage(emptyList)).ToList();

            // Check
            Assert.Equal(emptyList.Count, resultList.Count);
            for (int i = 0; i < count; i++)
            {
                Assert.Equal(emptyList[i].GradId, resultList[i].GradId);
                Assert.Equal(emptyList[i].UserDate, resultList[i].UserDate);
                Assert.Equal(emptyList[i].CardType, resultList[i].CardType);
                Assert.Equal(emptyList[i].UsedLocation, resultList[i].UsedLocation);
                Assert.Equal(emptyList[i].MoneyUsed, resultList[i].MoneyUsed);
                Assert.Equal(emptyList[i].UsedType, resultList[i].UsedType);
            }
        }

        [Fact(DisplayName = "SaveMoneyUsage Throws exception when same key applied.")]
        public async void Is_SaveMoneyUsage_Throws_Exception_When_Duplicated_Key()
        {
            var mockMoneyUsage = new MoneyUsage
            {
                GradId = Guid.NewGuid().ToString(),
                UserDate = DateTime.Now,
                CardType = Guid.NewGuid().ToString(),
                UsedLocation = Guid.NewGuid().ToString(),
                MoneyUsed = 21600,
                UsedType = Guid.NewGuid().ToString()
            };
            // Save Once
            await _moneyRepository.SaveMoneyUsage(new List<MoneyUsage> {mockMoneyUsage});
            
            // Check
            await Assert.ThrowsAnyAsync<Exception>(() => _moneyRepository.SaveMoneyUsage(new List<MoneyUsage> {mockMoneyUsage}));
        }
    }
}