namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class CategoryExpenseStatDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public decimal TotalExpense { get; set; }
} 