namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
} 