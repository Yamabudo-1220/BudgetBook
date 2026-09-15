using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BudgetBook.Models;

public class Transaction
{
	public int Id { get; set; }

	[Range(0.01, 999999.99, ErrorMessage = "Der Betrag muss größer als 0 sein.")]
	public decimal Amount { get; set; }

	[DataType(DataType.Date)]
	public DateTime BookingDate { get; set; } = DateTime.Today;

	public TransactionType Type { get; set; }

	[StringLength(200)]
	public string? Description { get; set; }

	public int CategoryId { get; set; }
	public Category? Category { get; set; }

	[Required]
	public string UserId { get; set; } = string.Empty;
	public IdentityUser? User { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}