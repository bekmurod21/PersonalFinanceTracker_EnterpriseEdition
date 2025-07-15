namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class CategoryExpenseStatDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal TotalExpense { get; set; }
} 