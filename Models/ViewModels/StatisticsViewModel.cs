using System.ComponentModel.DataAnnotations;
using BudgetBook.Models;

namespace BudgetBook.Models.ViewModels;

public class StatisticsViewModel
{
    [Display(Name = "Einnahmen")]
    public decimal TotalIncome { get; set; }

    [Display(Name = "Ausgaben")]
    public decimal TotalExpense { get; set; }

    [Display(Name = "Saldo")]
    public decimal Balance => TotalIncome - TotalExpense;

    public List<CategorySum> CategorySums { get; set; } = new();

    // Größte Ausgaben-Kategorie (laut Aufgaben-Erwartung)
    public CategorySum? TopCategory => CategorySums
        .Where(c => c.Type == TransactionType.Expense)
        .OrderByDescending(c => c.Total)
        .FirstOrDefault();
}

public class CategorySum
{
    public string CategoryName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Total { get; set; }
}