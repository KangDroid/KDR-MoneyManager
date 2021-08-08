using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using MoneyManager.Communication;
using MoneyManager.Models;
using MoneyManager.Repository;
using MoneyManager.Service;
using Moq;
using Xunit;

namespace MoneyManagerTest.Service
{
    public class MoneyUsageServiceTest
    {
        private readonly IMoneyUsageService _moneyUsageService;
        private readonly Mock<IMoneyUsageRepository> _moneyRepository;
        
        public MoneyUsageServiceTest()
        {
            _moneyRepository = new Mock<IMoneyUsageRepository>();
            _moneyUsageService = new MoneyUsageService(_moneyRepository.Object);
        }

        private List<MoneyUsage> GetRandomMoneyUsage(int count)
        {
            var moneyList = new List<MoneyUsage>();
            for (int i = 0; i < count; i++)
            {
                moneyList.Add(
                    new MoneyUsage
                    {
                        GradId = Guid.NewGuid().ToString(),
                        CardType = Guid.NewGuid().ToString(),
                        UsedLocation = Guid.NewGuid().ToString(),
                        MoneyUsed = 10000,
                        UsedType = Guid.NewGuid().ToString(),
                        UserDate = DateTime.Now
                    });
            }

            return moneyList;
        }

        [Fact(DisplayName = "GetMoneyUsageFromCsv: GetMoneyUsageFromCsv should parse given csv very well.")]
        public async void Is_GetMoneyUsageFromCsv_Works_Well()
        {
            var testMoneyUsage = GetRandomMoneyUsage(1000);

            var testFilePath = Path.Combine(Path.GetTempPath(), "test.csv");
            await using (var writer = new StreamWriter(testFilePath))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(testMoneyUsage);
            }
            
            // Open Stream
            await using (var stream = File.OpenRead(testFilePath))
            {
                var testFormFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                // Do
                var executionResult = await _moneyUsageService.GetMoneyUsageFromCsv(testFormFile);
                Assert.Equal(ResultInfo.Success, executionResult.Status);

                var result = executionResult.ResultObject;
                Assert.Equal(1000, result.Count);
            
                foreach (var eachResult in result)
                {
                    Assert.NotNull(testMoneyUsage.Find(money => money.GradId == eachResult.GradId));
                }
            }
            
            // Remove[Cleanup]
            File.Delete(testFilePath);
        }

        [Fact(DisplayName =
            "GetMoneyUsageFromCsv: GetMoneyUsageFromCsv should return BadCSVHeader when input file is not csv.")]
        public async void Is_GetMoneyUsageFromCsv_Throws_Exception_Well()
        {
            var tempFilePath = Path.GetTempFileName();
            await using (var streamReader = File.CreateText(tempFilePath))
            {
                await streamReader.WriteLineAsync("Test File");
            }
            
            // Open Stream
            await using (var stream = File.OpenRead(tempFilePath))
            {
                var testFormFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
        
                // Do
                var result = await _moneyUsageService.GetMoneyUsageFromCsv(testFormFile);
                Assert.Equal(ResultInfo.BadCsvHeader, result.Status);
            }
        }

        [Theory(DisplayName =
            "GetMoneyUsageFromCsv: GetMoneyUsageFromCsv should return BadCSVHeader whatever file is.")]
        [InlineData("Test File")]
        [InlineData("1")]
        [InlineData("1, 2, 3")]
        [InlineData("UsedType, MoneyUsed")]
        public async void Is_GetMoneyUsageFromCsv_Returns_BadCsvHeader_No_Matter_What(string target)
        {
            var tempFilePath = Path.GetTempFileName();
            await using (var streamReader = File.CreateText(tempFilePath))
            {
                await streamReader.WriteLineAsync(target);
            }
            
            // Open Stream
            await using (var stream = File.OpenRead(tempFilePath))
            {
                var testFormFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
        
                // Do
                var result = await _moneyUsageService.GetMoneyUsageFromCsv(testFormFile);
                Assert.Equal(ResultInfo.BadCsvHeader, result.Status);
            }
        }
    }
}