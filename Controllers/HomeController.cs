using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LabelSite.Infrastructure;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml;
using LabelSite.Models;

namespace LabelSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var salesData = await _context.SalesDatas
                                         .Include(sd => sd.User)
                                         .GroupBy(sd => new { sd.User.UserName, sd.SaleDate.Year, sd.SaleDate.Month })
                                         .Select(g => new
                                         {
                                             UserName = g.Key.UserName,
                                             Year = g.Key.Year,
                                             Month = g.Key.Month,
                                             TotalSales = g.Sum(sd => sd.Amount)
                                         })
                                         .OrderBy(g => g.Year)
                                         .ThenBy(g => g.Month)
                                         .ToListAsync();

                return View(salesData);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User); // Получение текущего пользователя
                var salesData = await _context.SalesDatas
                    .Include(sd => sd.User)
                    .Where(sd => sd.UserId == user.Id) // Ограничение данных только до продаж текущего пользователя
                    .GroupBy(sd => new { sd.User.UserName, sd.SaleDate.Year, sd.SaleDate.Month })
                    .Select(g => new
                    {
                        UserName = g.Key.UserName,
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalSales = g.Sum(sd => sd.Amount)
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync();
                return View(salesData);
            }
               
        }
        [Authorize]
        [Authorize]
        public async Task<IActionResult> GenerateReport()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var salesData = await _context.SalesDatas
                                          .Include(sd => sd.User)
                                          .OrderBy(sd => sd.SaleDate)
                                          .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Отчёт");

                // Add header row
                worksheet.Cells[1, 1].Value = "Дата продажи";
                worksheet.Cells[1, 2].Value = "Сумма";
                worksheet.Cells[1, 3].Value = "Имя пользователя";
                worksheet.Cells[1, 4].Value = "Количество продаж";
                worksheet.Cells[1, 5].Value = "Цель продаж";
                worksheet.Cells[1, 6].Value = "Процент выполнения продаж";
                worksheet.Cells[1, 7].Value = "Начало месяца";
                worksheet.Cells[1, 8].Value = "Конец месяца";


                // Add data rows
                for (int i = 0; i < salesData.Count; i++)
                {
                    var sale = salesData[i];
                    worksheet.Cells[i + 2, 1].Value = sale.SaleDate.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 2, 2].Value = sale.Amount;
                    worksheet.Cells[i + 2, 3].Value = sale.User.UserName;
                    worksheet.Cells[i + 2, 4].Value = sale.SalesCount;
                    worksheet.Cells[i + 2, 5].Value = sale.SalesGoal;
                    worksheet.Cells[i + 2, 6].Value = sale.SalesPercentage / 100; // Процент выполнения продаж в виде десятичной дроби (например, 0.75 для 75%)
                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "0.00%";
                    worksheet.Cells[i + 2, 7].Value = sale.MonthStart.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 2, 8].Value = sale.MonthEnd.ToString("dd.MM.yyyy");
                }

                
                // Save the Excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var content = stream.ToArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Отчёт.xlsx";

                return File(content, contentType, fileName);
            }
         }

    }
}
