using BudgetBook.Data;
using BudgetBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<IActionResult> Index(DateTime? from, DateTime? to, TransactionType? type, int? categoryId)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.Transactions
            .Where(t => t.UserId == userId);

        if (from.HasValue)
            query = query.Where(t => t.BookingDate >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.BookingDate <= to.Value);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        var viewModel = await StatisticsService.BuildAsync(query);

        // Filter-Auswahl ans Formular zurückgeben
        viewModel.DateFrom = from;
        viewModel.DateTo = to;
        viewModel.FilterType = type;
        viewModel.FilterCategoryId = categoryId;
        viewModel.Categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();

        return View(viewModel);
    }
}