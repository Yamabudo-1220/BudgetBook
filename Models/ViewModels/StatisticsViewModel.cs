using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models.ViewModels;

public class StatisticsViewModel
{
    [Display(Name = "Einnahmen")]
    public decimal TotalIncome { get; set; }

    [Display(Name = "Ausgaben")]
    public decimal TotalExpense { get; set;}

    [Display(Name = "Saldo")]
    public decimal Balance => TotalIncome - TotalExpense;
}