using BudgetBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetBook.Data;

public class ApplicationDbContext : IdentityDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Category> Categories => Set<Category>();
	public DbSet<Transaction> Transactions => Set<Transaction>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Transaction>()
			.Property(t => t.Amount)
			.HasPrecision(18, 2);

		builder.Entity<Transaction>()
			.HasOne(t => t.Category)
			.WithMany(c => c.Transactions)
			.HasForeignKey(t => t.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Transaction>()
			.HasOne(t => t.User)
			.WithMany()
			.HasForeignKey(t => t.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<Category>().HasData(
			new Category { Id = 1, Name = "Gehalt", Type = TransactionType.Income, IsActive = true },
			new Category { Id = 2, Name = "Sonstige Einnahmen", Type = TransactionType.Income, IsActive = true },
			new Category { Id = 3, Name = "Lebensmittel", Type = TransactionType.Expense, IsActive = true },
			new Category { Id = 4, Name = "Miete", Type = TransactionType.Expense, IsActive = true },
			new Category { Id = 5, Name = "Freizeit", Type = TransactionType.Expense, IsActive = true },
			new Category { Id = 6, Name = "Verkehr", Type = TransactionType.Expense, IsActive = true }
		);
	}
}