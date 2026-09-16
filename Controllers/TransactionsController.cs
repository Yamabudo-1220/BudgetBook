using BudgetBook.Data;
using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TransactionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Transactions
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.BookingDate)
            .ThenByDescending(t => t.Id)
            .ToListAsync();

        return View(transactions);
    }

    // GET: Transactions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
            return NotFound();

        var transaction = await FindOwnTransactionAsync(id.Value, includeCategory: true);
        if (transaction is null)
            return NotFound();

        return View(transaction);
    }

    // GET: Transactions/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new TransactionFormViewModel
        {
            Categories = await BuildCategoryListAsync()
        };
        return View(viewModel);
    }

    // POST: Transactions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransactionFormViewModel viewModel)
    {
        await ValidateCategoryAsync(viewModel);

        if (!ModelState.IsValid)
        {
            viewModel.Categories = await BuildCategoryListAsync();
            return View(viewModel);
        }

        var transaction = new Transaction
        {
            Amount = viewModel.Amount,
            BookingDate = viewModel.BookingDate,
            Type = viewModel.Type,
            CategoryId = viewModel.CategoryId,
            Description = viewModel.Description,
            UserId = _userManager.GetUserId(User)!,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Transactions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        var transaction = await FindOwnTransactionAsync(id.Value);
        if (transaction is null)
            return NotFound();

        var viewModel = new TransactionFormViewModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            BookingDate = transaction.BookingDate,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            Description = transaction.Description,
            Categories = await BuildCategoryListAsync()
        };

        return View(viewModel);
    }

    // POST: Transactions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TransactionFormViewModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        await ValidateCategoryAsync(viewModel);

        if (!ModelState.IsValid)
        {
            viewModel.Categories = await BuildCategoryListAsync();
            return View(viewModel);
        }

        var transaction = await FindOwnTransactionAsync(id);
        if (transaction is null)
            return NotFound();

        transaction.Amount = viewModel.Amount;
        transaction.BookingDate = viewModel.BookingDate;
        transaction.Type = viewModel.Type;
        transaction.CategoryId = viewModel.CategoryId;
        transaction.Description = viewModel.Description;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Transactions/Delete/5  (Bestätigungsseite)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
            return NotFound();

        var transaction = await FindOwnTransactionAsync(id.Value, includeCategory: true);
        if (transaction is null)
            return NotFound();

        return View(transaction);
    }

    // POST: Transactions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await FindOwnTransactionAsync(id);
        if (transaction is not null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Lädt eine Buchung nur, wenn sie der angemeldeten Person gehört – sonst null.
    private async Task<Transaction?> FindOwnTransactionAsync(int id, bool includeCategory = false)
    {
        var userId = _userManager.GetUserId(User);

        IQueryable<Transaction> query = _context.Transactions;
        if (includeCategory)
            query = query.Include(t => t.Category);

        return await query.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    // Geschäftsregel: Kategorie muss existieren, aktiv sein und zum Typ passen.
    private async Task ValidateCategoryAsync(TransactionFormViewModel viewModel)
    {
        var category = await _context.Categories.FindAsync(viewModel.CategoryId);

        if (category is null || !category.IsActive)
        {
            ModelState.AddModelError(nameof(viewModel.CategoryId), "Bitte eine gültige Kategorie wählen.");
            return;
        }

        if (category.Type != viewModel.Type)
        {
            var expected = viewModel.Type == TransactionType.Income ? "Einnahmen" : "Ausgaben";
            ModelState.AddModelError(nameof(viewModel.CategoryId),
                $"Die Kategorie „{category.Name}“ passt nicht zum Typ. Bitte eine {expected}-Kategorie wählen.");
        }
    }

    private async Task<IEnumerable<CategoryOption>> BuildCategoryListAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOption
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            })
            .ToListAsync();
    }
}