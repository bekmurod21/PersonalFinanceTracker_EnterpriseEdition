using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;

public static class ExcelExportHelper
{
    public static byte[] ExportTopCategoryExpensesToExcel(List<CategoryExpenseStatDto> stats)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Top Categories");
        worksheet.Cell(1, 1).Value = "Kategoriya nomi";
        worksheet.Cell(1, 2).Value = "Umumiy xarajat";

        for (int i = 0; i < stats.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = stats[i].Name;
            worksheet.Cell(i + 2, 2).Value = stats[i].TotalExpense;
        }
        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static byte[] ExportMonthlyTrendToExcel(List<MonthlyTrendDto> trends)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Trend");
        worksheet.Cell(1, 1).Value = "Yil";
        worksheet.Cell(1, 2).Value = "Oy";
        worksheet.Cell(1, 3).Value = "Daromad";
        worksheet.Cell(1, 4).Value = "Xarajat";

        for (int i = 0; i < trends.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = trends[i].Year;
            worksheet.Cell(i + 2, 2).Value = trends[i].Month;
            worksheet.Cell(i + 2, 3).Value = trends[i].Income;
            worksheet.Cell(i + 2, 4).Value = trends[i].Expense;
        }
        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
} 