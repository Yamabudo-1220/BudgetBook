using System.ComponentModel.DataAnnotations;

namespace BudgetBook.Models.ViewModels;

public class TransactionFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Betrag")]
    [Range(0.01, 999999.99, ErrorMessage = "Der Betrag muss größer als 0 sein.")]
    public decimal Amount { get; set; }

    [Display(Name = "Buchungsdatum")]
    [DataType(DataType.Date)]
    public DateTime BookingDate { get; set; } = DateTime.Today;

    [Display(Name = "Typ")]
    public TransactionType Type { get; set; }

    [Display(Name = "Kategorie")]
    [Range(1, int.MaxValue, ErrorMessage = "Bitte wählen Sie eine Kategorie aus.")]
    public int CategoryId { get; set; }

    [Display(Name = "Beschreibung")]
    [StringLength(200, ErrorMessage = "Maximal 200 Zeichen.")]
    public string? Description { get; set; }

    public IEnumerable<CategoryOption> Categories { get; set; } = new List<CategoryOption>();
}

public class CategoryOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}