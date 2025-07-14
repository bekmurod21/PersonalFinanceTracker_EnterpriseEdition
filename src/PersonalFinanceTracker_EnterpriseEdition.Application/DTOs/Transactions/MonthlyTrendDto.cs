namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class MonthlyTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
} 