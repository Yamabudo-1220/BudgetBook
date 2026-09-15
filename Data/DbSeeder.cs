using BudgetBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data;

public static class DbSeeder
{
	public static async Task SeedSampleTransactionsAsync(ApplicationDbContext context)
	{
		if (await context.Transactions.AnyAsync())
			return;

		var firstUserId = await context.Users
			.OrderBy(u => u.UserName)
			.Select(u => u.Id)
			.FirstOrDefaultAsync();

		if (firstUserId is null)
			return;

		var today = DateTime.Today;

		var samples = new List<Transaction>
		{
			new() { Amount = 2400.00m, BookingDate = today.AddDays(-20), Type = TransactionType.Income, CategoryId = 1, Description = "Septembergehalt", UserId = firstUserId },
			new() { Amount = 850.00m, BookingDate = today.AddDays(-19), Type = TransactionType.Expense, CategoryId = 4, Description = "Miete September", UserId = firstUserId },
			new() { Amount = 63.40m, BookingDate = today.AddDays(-10), Type = TransactionType.Expense, CategoryId = 3, Description = "Wocheneinkauf", UserId = firstUserId },
			new() { Amount = 29.90m, BookingDate = today.AddDays(-4), Type = TransactionType.Expense, CategoryId = 5, Description = "Kino", UserId = firstUserId },
			new() { Amount = 49.00m, BookingDate = today.AddDays(-1), Type = TransactionType.Expense, CategoryId = 6, Description = "Monatskarte", UserId = firstUserId }
		};

		context.Transactions.AddRange(samples);
		await context.SaveChangesAsync();
	}
}