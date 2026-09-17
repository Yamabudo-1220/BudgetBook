using BudgetBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data;

public static class DbSeeder
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Beide Rollen anlegen, falls sie fehlen.
        foreach (var role in new[] { AdminRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Ersten registrierten Benutzer zum Admin machen, alle anderen zu Usern.
        var firstUser = await userManager.Users
            .OrderBy(u => u.UserName)
            .FirstOrDefaultAsync();

        if (firstUser is not null && !await userManager.IsInRoleAsync(firstUser, AdminRole))
            await userManager.AddToRoleAsync(firstUser, AdminRole);

        foreach (var user in await userManager.Users.ToListAsync())
        {
            var isAdmin = firstUser is not null && user.Id == firstUser.Id;
            var targetRole = isAdmin ? AdminRole : UserRole;

            if (!await userManager.IsInRoleAsync(user, targetRole))
                await userManager.AddToRoleAsync(user, targetRole);
        }
    }

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