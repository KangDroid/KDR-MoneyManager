using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyManager.Communication;
using MoneyManager.Models;
using MoneyManager.Service;

namespace MoneyManager.Controllers
{
    [ApiController]
    [Route("api/money")]
    public class MoneyController: ControllerBase
    {
        private readonly IMoneyUsageService _moneyUsageService;

        public MoneyController(IMoneyUsageService moneyUsageService)
        {
            _moneyUsageService = moneyUsageService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveMoneyUsageFromCsv(IFormFile file)
        {
            // Get Money Usage
            var moneyUsages = await _moneyUsageService.GetMoneyUsageFromCsv(file);

            if (moneyUsages.Status == ResultInfo.BadCsvHeader)
            {
                // Bad CSV Header
                return BadRequest();
            }
            
            // Save it
            var saveResult = await _moneyUsageService.SaveMoneyUsage(moneyUsages.ResultObject);

            if (saveResult.Status == ResultInfo.DbUpdateError)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return Ok(saveResult.ResultObject);
        }
    }
}