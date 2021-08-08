using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using MoneyManager.Communication;
using MoneyManager.Models;
using MoneyManager.Repository;

namespace MoneyManager.Service
{
    public class MoneyUsageService : IMoneyUsageService
    {
        private readonly IMoneyUsageRepository _moneyUsageRepository;

        public MoneyUsageService(IMoneyUsageRepository moneyUsageRepository)
        {
            _moneyUsageRepository = moneyUsageRepository;
        }

        public async Task<Result<List<MoneyUsage>>> GetMoneyUsageFromCsv(IFormFile csvFile)
        {
            // Save CSV File to File
            var savedPath = await WriteFileToTempStorage(csvFile);
            
            // Convert CSV to List
            List<MoneyUsage> convertedList;
            using (var csv = new CsvReader(new StreamReader(savedPath), CultureInfo.InvariantCulture))
            {
                try
                {
                    convertedList = csv.GetRecords<MoneyUsage>().ToList();
                }
                catch (HeaderValidationException hve)
                {
                    return new Result<List<MoneyUsage>>
                    {
                        FailureMessage = "Cannot parse CSV!",
                        DetailedFailureMessage = "Cannot parse csv file. Perhaps you uploaded wrong file.",
                        Status = ResultInfo.BadCsvHeader
                    };
                }
            }
            
            // Cleanup
            File.Delete(savedPath);

            return new Result<List<MoneyUsage>>
            {
                ResultObject = convertedList,
                Status = ResultInfo.Success
            };
        }

        public async Task<Result<List<MoneyUsage>>> SaveMoneyUsage(List<MoneyUsage> moneyUsages)
        {
            List<MoneyUsage> targetList;
            try
            {
                targetList = (await _moneyUsageRepository.SaveMoneyUsage(moneyUsages)).ToList();
            }
            catch (Exception e)
            {
                return new Result<List<MoneyUsage>>
                {
                    Status = ResultInfo.DbUpdateError,
                    FailureMessage = "Failed to update db with data!",
                    DetailedFailureMessage = $"StackTrace: {e.Message}"
                };
            }

            return new Result<List<MoneyUsage>>
            {
                Status = ResultInfo.Success,
                ResultObject = targetList
            };
        }

        private async Task<string> WriteFileToTempStorage(IFormFile csvFile)
        {
            // First, Copy Input FormFile to somewhere else
            var tmpFilePath = Path.GetTempFileName();
            await using (var stream = File.Create(tmpFilePath))
            {
                await csvFile.CopyToAsync(stream);
            }

            return tmpFilePath;
        }
    }
}