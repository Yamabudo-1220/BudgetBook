using BudgetBook.Models;

namespace BudgetBook.Models.ViewModels;

public class AdminUserRow
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
}

public class AdminUserDetailViewModel
{
    public string Email { get; set; } = string.Empty;
    public List<Transaction> Transactions { get; set; } = new();
}