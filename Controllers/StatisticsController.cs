using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public StatisticsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var ownTransactions = _context.Transactions
            .Where(t => t.UserId == userId);

        var categorySums = await ownTransactions
            .GroupBy(t => new { t.Category!.Name, t.Type })
            .Select(g => new CategorySum
            {
                CategoryName = g.Key.Name,
                Type = g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        var viewModel = new StatisticsViewModel
        {
            TotalIncome = await ownTransactions
                .Where(t => t.Type == TransactionType.Income)
                .SumAsync(t => t.Amount),

            TotalExpense = await ownTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount),

            CategorySums = categorySums
                .OrderBy(c => c.Type)
                .ThenByDescending(c => c.Total)
                .ToList()
        };

        return View(viewModel);
    }
}