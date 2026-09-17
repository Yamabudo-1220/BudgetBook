using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers;

[Authorize(Roles = DbSeeder.AdminRole)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Admin  – Benutzerübersicht mit Kennzahlen
    public async Task<IActionResult> Index()
    {
        // Kennzahlen je Benutzer aus den Buchungen berechnen (in der DB gruppieren).
        var sums = await _context.Transactions
            .GroupBy(t => t.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Count = g.Count(),
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .ToListAsync();

        // Alle Benutzer holen (auch solche ganz ohne Buchungen).
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => new { u.Id, u.Email })
            .ToListAsync();

        var rows = users.Select(u =>
        {
            var s = sums.FirstOrDefault(x => x.UserId == u.Id);
            return new AdminUserRow
            {
                UserId = u.Id,
                Email = u.Email ?? "(ohne E-Mail)",
                TransactionCount = s?.Count ?? 0,
                TotalIncome = s?.Income ?? 0m,
                TotalExpense = s?.Expense ?? 0m
            };
        }).ToList();

        return View(rows);
    }

    // GET: Admin/UserTransactions/<userId>  – Buchungen einer Person
    public async Task<IActionResult> UserTransactions(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == id)
            .OrderByDescending(t => t.BookingDate)
            .ThenByDescending(t => t.Id)
            .ToListAsync();

        var viewModel = new AdminUserDetailViewModel
        {
            Email = user.Email ?? "(ohne E-Mail)",
            Transactions = transactions
        };

        return View(viewModel);
    }

    // GET: Admin/UserStatistics/<userId>  – Statistik einer Person
    public async Task<IActionResult> UserStatistics(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        var transactions = _context.Transactions
            .Where(t => t.UserId == id);

        var viewModel = await StatisticsService.BuildAsync(transactions);

        ViewData["Title"] = "Statistik von " + user.Email;
        ViewData["AdminHeading"] = "Statistik von " + user.Email;

        return View("~/Views/Statistics/Index.cshtml", viewModel);
    }
}