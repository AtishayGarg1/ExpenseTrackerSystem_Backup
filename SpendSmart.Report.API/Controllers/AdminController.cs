using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSmart.Report.API.Data;

namespace SpendSmart.Report.API.Controllers
{
    [ApiController]
    [Route("api/reports/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ReportDbContext _context;

        public AdminController(ReportDbContext context)
        {
            _context = context;
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
            var totalIncome = await _context.Incomes.SumAsync(i => i.Amount);
            
            var platformStatus = "Healthy"; 

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalExpenses = totalExpenses,
                TotalIncome = totalIncome,
                PlatformStatus = platformStatus,
                LastUpdated = DateTime.UtcNow
            });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToListAsync();
            return Ok(logs);
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses()
        {
            var expenses = await _context.Expenses
                .Join(_context.Users, e => e.UserId, u => u.UserId, (e, u) => new
                {
                    e.ExpenseId,
                    e.Amount,
                    e.Currency,
                    e.Description,
                    e.Date,
                    UserName = u.FullName,
                    UserEmail = u.Email
                })
                .OrderByDescending(e => e.Date)
                .Take(100)
                .ToListAsync();
            return Ok(expenses);
        }

        [HttpGet("incomes")]
        public async Task<IActionResult> GetIncomes()
        {
            var incomes = await _context.Incomes
                .Join(_context.Users, i => i.UserId, u => u.UserId, (i, u) => new
                {
                    i.IncomeId,
                    i.Amount,
                    i.Currency,
                    i.Source,
                    i.Description,
                    i.Date,
                    UserName = u.FullName,
                    UserEmail = u.Email
                })
                .OrderByDescending(i => i.Date)
                .Take(100)
                .ToListAsync();
            return Ok(incomes);
        }
    }
}
