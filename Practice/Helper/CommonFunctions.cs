using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Practice.Models.ViewModels;

namespace Practice.Helper
{
    public static class CommonFunctions
    {
        public static int CountWorkingDays(DateTime start1, DateTime start2, DateTime? end1, DateTime end2)
        {
            DateTime start = GetMaxDate(start1, start2);
            DateTime end = GetMinDate(end1, end2);

            int count = 0;

            if (start > end)
                return count;

            while (start < end)
            {
                start = start.AddDays(1);
                if ((start.DayOfWeek != DayOfWeek.Saturday) && (start.DayOfWeek != DayOfWeek.Sunday))
                    count++;
            }

            return count;
        }

        public static DateTime GetMinDate(DateTime? date1, DateTime date2)
        {
            if (date1 == null)
                return date2;

            if (date1 <= date2)
                return new DateTime(0 + date1.Value.Year, 0 + date1.Value.Month, 0 + date1.Value.Day);

            return date2;
        }

        public static DateTime GetMaxDate(DateTime date1, DateTime date2)
        {
            if (date1 >= date2)
                return date1;

            return date2;
        }

        public static IActionResult GetReport(ReportViewModel page)
        {
            // https://alekseev74.ru/lessons/show/aspnet-mvc/ms-excel
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet = workbook.Worksheets.Add(page.ReportType);
                worksheet.Row(1).Style.Font.Bold = true;
                int k = 1;

                foreach (var item in page.Report[0])
                {
                    worksheet.Cell(1, k).Value = item.Key;
                    k++;
                }

                for (int i = 2; i <= page.Report.Count + 1; i++)
                {
                    int j = 1;

                    foreach (var item in page.Report[i - 2].Values)
                    {
                        worksheet.Cell(i, j).Value = item;
                        j++;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"report_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
    }
}
