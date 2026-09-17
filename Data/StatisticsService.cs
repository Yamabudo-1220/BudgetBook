using BudgetBook.Models;
using BudgetBook.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data;

public static class StatisticsService
{
    public static async Task<StatisticsViewModel> BuildAsync(IQueryable<Transaction> transactions)
    {
        var categorySums = await transactions
            .GroupBy(t => new { t.Category!.Name, t.Type })
            .Select(g => new CategorySum
            {
                CategoryName = g.Key.Name,
                Type = g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        var monthlySums = await transactions
            .GroupBy(t => new { t.BookingDate.Year, t.BookingDate.Month })
            .Select(g => new MonthSum
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Sum(t => t.Type == TransactionType.Income ? t.Amount : 0m),
                Expense = g.Sum(t => t.Type == TransactionType.Expense ? t.Amount : 0m)
            })
            .ToListAsync();

        return new StatisticsViewModel
        {
            TotalIncome = await transactions
                .Where(t => t.Type == TransactionType.Income)
                .SumAsync(t => t.Amount),

            TotalExpense = await transactions
                .Where(t => t.Type == TransactionType.Expense)
                .SumAsync(t => t.Amount),

            CategorySums = categorySums
                .OrderBy(c => c.Type)
                .ThenByDescending(c => c.Total)
                .ToList(),

            MonthlySums = monthlySums
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList()
        };
    }
}