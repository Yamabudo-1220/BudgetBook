using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models;

public class Category
{
	public int Id { get; set; }

	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	public TransactionType Type { get; set; }

	public bool IsActive { get; set; } = true;

	public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}