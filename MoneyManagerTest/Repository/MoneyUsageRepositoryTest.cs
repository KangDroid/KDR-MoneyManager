using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoneyManager.Models;
using MoneyManager.Repository;
using Xunit;

namespace MoneyManagerTest.Repository
{
    public class MoneyUsageRepositoryTest: IDisposable
    {
        private readonly IMoneyUsageRepository _moneyRepository;
        private readonly MoneyContext _moneyContext;

        public void Dispose()
        {
            // Cleanup
            _moneyContext.Database.EnsureCreated();
            _moneyContext.MoneyUsages.RemoveRange(_moneyContext.MoneyUsages);
            _moneyContext.SaveChanges();
        }

        public MoneyUsageRepositoryTest()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            var connectionString = configuration.GetConnectionString("MoneyManagementDb");
            var dbOptions = new DbContextOptionsBuilder<MoneyContext>().UseSqlServer("Server=localhost; Database=kdr-test; UID=SA; Password=testPassword@;").Options;
            _moneyContext = new MoneyContext(dbOptions);
            
            // Cleanup
            _moneyContext.Database.EnsureCreated();
            _moneyContext.MoneyUsages.RemoveRange(_moneyContext.MoneyUsages);
            _moneyContext.SaveChanges();
            
            // New Repository
            _moneyRepository = new MoneyUsageRepository(_moneyContext);
        }

        private List<MoneyUsage> CreateRandomMoneyList(int count)
        {
            var emptyList = new List<MoneyUsage>();
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

            return emptyList;
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
            var emptyList = CreateRandomMoneyList(count);
            
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

        [Fact(DisplayName = "SaveMoneyUsage will save updated item when same key applied.")]
        public async void Is_SaveMoneyUsage_Save_Updated_Item_When_Duplicated_Key()
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
            var result = await _moneyRepository.SaveMoneyUsage(new List<MoneyUsage> { mockMoneyUsage });
            Assert.Single(result);
        }

        [Theory(DisplayName = "GetMoneyUsage Get proper list well")]
        [InlineData(0)]
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
        public async void Is_GetMoneyUsage_Returns_Proper_List(int toRegister)
        {
            // Prep
            var moneyList = CreateRandomMoneyList(toRegister);
            await _moneyRepository.SaveMoneyUsage(moneyList);
            
            // Check
            var resultList = (await _moneyRepository.GetMoneyUsage()).ToList();
            Assert.Equal(toRegister, resultList.Count);
        }
    }
}