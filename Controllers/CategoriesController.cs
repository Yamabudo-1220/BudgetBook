using BudgetBook.Data;
using BudgetBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Controllers;

[Authorize(Roles = DbSeeder.AdminRole)]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Type)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Type,IsActive")] Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category is null)
            return NotFound();

        return View(category);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,IsActive")] Category category)
    {
        if (id != category.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(category);

        _context.Update(category);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}