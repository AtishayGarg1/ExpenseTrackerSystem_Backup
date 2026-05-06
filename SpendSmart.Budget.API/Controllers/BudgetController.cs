using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Budget.API.Services;
using System.Security.Claims;

namespace SpendSmart.Budget.API.Controllers
{
    [ApiController]
    [Route("api/budgets")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        public record CreateBudgetRequest(int? CategoryId, string Name, decimal LimitAmount, string Currency, string Period, DateTime StartDate, DateTime EndDate);
        public record UpdateBudgetRequest(int? CategoryId, string Name, decimal LimitAmount, string Period, DateTime StartDate, DateTime EndDate);
        public record InternalCheckRequest(int UserId, int CategoryId, decimal Amount);

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBudgetRequest request)
        {
            var budget = await _budgetService.CreateBudgetAsync(GetUserId(), request.CategoryId, request.Name, request.LimitAmount, request.Currency, request.Period, request.StartDate, request.EndDate);
            return Ok(budget);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return Ok(await _budgetService.GetActiveBudgetsAsync(GetUserId()));
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            return Ok(await _budgetService.GetOverBudgetAlertsAsync(GetUserId()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetRequest request)
        {
            var success = await _budgetService.UpdateBudgetAsync(id, GetUserId(), request.CategoryId, request.Name, request.LimitAmount, request.Period, request.StartDate, request.EndDate);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _budgetService.DeleteBudgetAsync(id, GetUserId());
            return success ? Ok() : BadRequest();
        }

        [HttpPost("internal/check")]
        [AllowAnonymous]
        public async Task<IActionResult> InternalCheck([FromBody] InternalCheckRequest request)
        {
            await _budgetService.CheckBudgetOnExpenseAsync(request.UserId, request.CategoryId, request.Amount);
            return Ok();
        }
    }
}
