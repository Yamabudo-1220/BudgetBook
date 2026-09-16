using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models;

public enum TransactionType
{
	[Display(Name = "Einnahme")]
	Income = 0,
	[Display(Name = "Ausgabe")]
	Expense = 1
}