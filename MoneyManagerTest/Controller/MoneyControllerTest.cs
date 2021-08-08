using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CsvHelper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyManager;
using MoneyManager.Communication;
using MoneyManager.Controllers;
using MoneyManager.Models;
using MoneyManager.Repository;
using MoneyManager.Service;
using Moq;
using Xunit;

namespace MoneyManagerTest.Controller
{
    public class MoneyControllerTest: IDisposable
    {
        private HttpClient _httpClient;
        private TestServer _testServer;

        public void Dispose()
        {
            var dbOptions = new DbContextOptionsBuilder<MoneyContext>().UseSqlServer("Server=localhost; Database=kdr-test; UID=SA; Password=testPassword@;").Options;
            var context = new MoneyContext(dbOptions);
            
            // Cleanup
            context.Database.EnsureCreated();
            context.MoneyUsages.RemoveRange(context.MoneyUsages);
            context.SaveChanges();
        }

        private TestServer InitializeServer(Func<IServiceCollection, object> serviceLambda)
        {
            var dbOptions = new DbContextOptionsBuilder<MoneyContext>().UseSqlServer("Server=localhost; Database=kdr-test; UID=SA; Password=testPassword@;").Options;
            var context = new MoneyContext(dbOptions);
            
            // Cleanup
            context.Database.EnsureCreated();
            context.MoneyUsages.RemoveRange(context.MoneyUsages);
            context.SaveChanges();
            
            return new TestServer(
                WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup>().ConfigureServices(services =>
                    {
                        serviceLambda?.Invoke(services);
                    }));
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

        [Fact(DisplayName = "POST /api/money should save money usage to db well.")]
        public async void Is_SaveMoneyUsageFromCsv_Returns_Well()
        {
            // Initialize Server
            _testServer = InitializeServer(null);
            _httpClient = _testServer.CreateClient();
            
            var testMoneyUsage = GetRandomMoneyUsage(1000);

            var testFilePath = Path.Combine(Path.GetTempPath(), "test.csv");
            await using (var writer = new StreamWriter(testFilePath))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(testMoneyUsage);
            }

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(File.OpenRead(testFilePath))
            {
                Headers =
                {
                    ContentLength = 100,
                    ContentType = new MediaTypeHeaderValue("text/csv")
                }
            }, "File", "test.csv");
            
            // Post it
            var result = await _httpClient.PostAsync("/api/money", content);
            
            // Check
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
        }

        [Theory(DisplayName = "POST /api/money should return BadRequest when input file is not csv.")]
        [InlineData("Test File")]
        [InlineData("1")]
        [InlineData("1, 2, 3")]
        [InlineData("UsedType, MoneyUsed")]
        public async void Is_SaveMoneyUsageFromCsv_Returns_BadRequest(string target)
        {
            // Initialize Server
            _testServer = InitializeServer(null);
            _httpClient = _testServer.CreateClient();
            
            var tempFilePath = Path.GetTempFileName();
            await using (var streamReader = File.CreateText(tempFilePath))
            {
                await streamReader.WriteLineAsync(target);
            }
            
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(File.OpenRead(tempFilePath))
            {
                Headers =
                {
                    ContentLength = 100,
                    ContentType = new MediaTypeHeaderValue("text/csv")
                }
            }, "File", "test.csv");
            
            // Post it
            var result = await _httpClient.PostAsync("/api/money", content);
            
            // Check
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact(DisplayName = "POST /api/money should return 500 when db - update error occurred.")]
        public async void Is_SaveMoneyUsageFromCsv_Returns_500()
        {
            // Non-Integration Test
            var mockService = new Mock<IMoneyUsageService>();
            var controller = new MoneyController(mockService.Object);
            var mockForm = new Mock<IFormFile>();
            
            // Setup
            mockService.Setup(a => a.GetMoneyUsageFromCsv(It.IsAny<IFormFile>()))
                .ReturnsAsync(new Result<List<MoneyUsage>>() { Status = ResultInfo.Success });
            mockService.Setup(a => a.SaveMoneyUsage(It.IsAny<List<MoneyUsage>>()))
                .ReturnsAsync(new Result<List<MoneyUsage>> { Status = ResultInfo.DbUpdateError });
            
            // Do
            var result = await controller.SaveMoneyUsageFromCsv(mockForm.Object);
            
            // Check
            Assert.Equal(StatusCodes.Status500InternalServerError, (result as StatusCodeResult).StatusCode);
        }
    }
}