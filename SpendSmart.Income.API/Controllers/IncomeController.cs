using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Income.API.Services;
using System.Security.Claims;

namespace SpendSmart.Income.API.Controllers
{
    [ApiController]
    [Route("api/incomes")]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        public record CreateIncomeRequest(string Source, decimal Amount, string Currency, string Description, DateTime Date, bool IsRecurring, string? RecurrenceType);
        public record UpdateIncomeRequest(string Source, decimal Amount, string Description, DateTime Date, bool IsRecurring, string? RecurrenceType);

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] CreateIncomeRequest request)
        {
            var income = await _incomeService.AddIncomeAsync(GetUserId(), request.Source, request.Amount, request.Currency, request.Description, request.Date, request.IsRecurring, request.RecurrenceType);
            return Ok(income);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetByUser()
        {
            return Ok(await _incomeService.GetIncomesByUserAsync(GetUserId()));
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal([FromQuery] int? month, [FromQuery] int? year)
        {
            if (month.HasValue && year.HasValue)
            {
                return Ok(new { Total = await _incomeService.GetTotalForMonthAsync(GetUserId(), month.Value, year.Value) });
            }
            return Ok(new { Total = await _incomeService.GetTotalIncomeAsync(GetUserId()) });
        }

        [HttpGet("net-balance")]
        public async Task<IActionResult> GetNetBalance()
        {
            return Ok(new { NetBalance = await _incomeService.GetNetBalanceAsync(GetUserId()) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIncomeRequest request)
        {
            var success = await _incomeService.UpdateIncomeAsync(id, GetUserId(), request.Source, request.Amount, request.Description, request.Date, request.IsRecurring, request.RecurrenceType);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _incomeService.DeleteIncomeAsync(id, GetUserId());
            return success ? Ok() : BadRequest();
        }
    }
}
