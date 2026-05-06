using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Expense.API.Services;
using System.Security.Claims;

namespace SpendSmart.Expense.API.Controllers
{
    [ApiController]
    [Route("api/expenses")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public record CreateExpenseRequest(int CategoryId, decimal Amount, string Currency, string Description, DateTime Date, string PaymentMode, string Tags, bool IsRecurring);
        public record UpdateExpenseRequest(int CategoryId, decimal Amount, string Description, DateTime Date, string PaymentMode, string Tags);

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] CreateExpenseRequest request)
        {
            var expense = await _expenseService.AddExpenseAsync(GetUserId(), request.CategoryId, request.Amount, request.Currency, request.Description, request.Date, request.PaymentMode, request.Tags, request.IsRecurring);
            return Ok(expense);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id, GetUserId());
            if (expense == null) return NotFound();
            return Ok(expense);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetByUser()
        {
            var expenses = await _expenseService.GetExpensesByUserAsync(GetUserId());
            return Ok(expenses);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            return Ok(await _expenseService.GetByCategoryAsync(GetUserId(), categoryId));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return Ok(await _expenseService.GetByDateRangeAsync(GetUserId(), start, end));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            return Ok(await _expenseService.SearchExpensesAsync(GetUserId(), q));
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal([FromQuery] int? month, [FromQuery] int? year)
        {
            if (month.HasValue && year.HasValue)
            {
                return Ok(new { Total = await _expenseService.GetTotalForMonthAsync(GetUserId(), month.Value, year.Value) });
            }
            return Ok(new { Total = await _expenseService.GetTotalByUserAsync(GetUserId()) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            var success = await _expenseService.UpdateExpenseAsync(id, GetUserId(), request.CategoryId, request.Amount, request.Description, request.Date, request.PaymentMode, request.Tags);
            return success ? Ok() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _expenseService.DeleteExpenseAsync(id, GetUserId());
            return success ? Ok() : BadRequest();
        }
    }
}
