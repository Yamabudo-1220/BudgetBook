using System.ComponentModel.DataAnnotations;
using BudgetBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public List<MonthSum> MonthlySums { get; set; } = new();

    public CategorySum? TopCategory => CategorySums
        .Where(c => c.Type == TransactionType.Expense)
        .OrderByDescending(c => c.Total)
        .FirstOrDefault();

    // Filter-Werte, damit das Formular die aktuelle Auswahl behält
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public TransactionType? FilterType { get; set; }
    public int? FilterCategoryId { get; set; }
    public List<SelectListItem> Categories { get; set; } = new();
}

public class CategorySum
{
    public string CategoryName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Total { get; set; }
}

public class MonthSum
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance => Income - Expense;
    public string Label => $"{Month:00}/{Year}";
}