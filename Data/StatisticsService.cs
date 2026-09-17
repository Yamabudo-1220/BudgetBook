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
                .ToList()
        };
    }
}