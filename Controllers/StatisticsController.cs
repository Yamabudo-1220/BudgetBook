using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using BudgetBook.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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

        var viewModel = new StatisticsViewModel
        {
            TotalIncome = await ownTransactions
                .Where(t => t.Type == TransactionType.Income)
                .SumAsync(t => t.Amount),

            TotalExpense = await ownTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount)
        };

        return View(viewModel);
    }
}