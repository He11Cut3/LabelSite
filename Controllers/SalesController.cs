using LabelSite.Infrastructure;
using LabelSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LabelSite.Controllers
{

    public class SalesController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public SalesController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var sales = await _context.SalesDatas
                                          .Include(sd => sd.User) // Загрузка данных о пользователе
                                          .Include(sd => sd.SalesProducts)
                                          .ThenInclude(sp => sp.Product)
                                          .ToListAsync();
                return View(sales);
            }
            else
            {
                var UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userSales = await _context.SalesDatas
                                              .Where(s => s.UserId == UserID)
                                              .Include(sd => sd.User) // Загрузка данных о пользователе
                                              .Include(sd => sd.SalesProducts)
                                              .ThenInclude(sp => sp.Product)
                                              .ToListAsync();
                return View(userSales);
            }
        }




        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var products = await _context.Products.ToListAsync();
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesData salesData, int[] selectedProducts, int[] quantities)
        {
            try
            {
                // Получаем текущего авторизованного пользователя
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    salesData.User = currentUser;

                    // Установка целей и дат
                    salesData.SalesGoal = 100000;
                    salesData.MonthStart = new DateTime(salesData.SaleDate.Year, salesData.SaleDate.Month, 1);
                    salesData.MonthEnd = salesData.MonthStart.AddMonths(1).AddDays(-1);

                    // Добавляем новую запись о продаже
                    _context.SalesDatas.Add(salesData);
                    await _context.SaveChangesAsync();

                    // Прикрепляем продукты к продаже и вычисляем сумму продажи и количество продаж
                    decimal totalAmount = 0;
                    int totalCount = 0;
                    for (int i = 0; i < selectedProducts.Length; i++)
                    {
                        var product = await _context.Products.FindAsync(selectedProducts[i]);
                        if (product != null)
                        {
                            var salesProduct = new SalesProduct
                            {
                                SalesDataId = salesData.SalesDataId,
                                ProductId = product.ProductId,
                                Quantity = quantities[i]
                            };
                            _context.SalesProducts.Add(salesProduct);

                            totalAmount += product.Price * quantities[i];
                            totalCount += quantities[i];
                        }
                    }

                    // Обновляем данные о продаже
                    salesData.Amount = totalAmount;
                    salesData.SalesCount = totalCount;

                    // Сохранение продуктов
                    await _context.SaveChangesAsync();

                    // Обновляем процент выполнения плана
                    var totalSalesForMonth = await _context.SalesDatas
                                                    .Where(sd => sd.SaleDate >= salesData.MonthStart && sd.SaleDate <= salesData.MonthEnd)
                                                    .SumAsync(sd => sd.Amount);

                    salesData.SalesPercentage = (totalSalesForMonth / salesData.SalesGoal) * 100;

                    // Сохранение изменений
                    _context.Update(salesData);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Текущий пользователь не найден.");
                    return View(salesData);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении данных о продаже: " + ex.Message);
                return View(salesData);
            }
        }


        // Добавление метода редактирования
        public async Task<IActionResult> Edit(int id)
        {
            var salesData = await _context.SalesDatas
                                            .Include(sd => sd.SalesProducts)
                                            .FirstOrDefaultAsync(sd => sd.SalesDataId == id);

            if (salesData == null)
            {
                return NotFound();
            }

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName");

            var products = await _context.Products.ToListAsync();
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");

            return View(salesData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime saleDate, int[] selectedProducts, int[] quantities)
        {
            var salesData = await _context.SalesDatas
                                            .Include(sd => sd.SalesProducts)
                                            .FirstOrDefaultAsync(sd => sd.SalesDataId == id);

            if (salesData == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновляем только нужные поля
                    salesData.SaleDate = saleDate;

                    // Обновляем MonthStart и MonthEnd
                    salesData.MonthStart = new DateTime(saleDate.Year, saleDate.Month, 1);
                    salesData.MonthEnd = salesData.MonthStart.AddMonths(1).AddDays(-1);

                    // Удаляем все связанные данные о продуктах
                    _context.SalesProducts.RemoveRange(salesData.SalesProducts);

                    decimal totalAmount = 0; // Инициализация переменной для подсчёта общей суммы продажи
                    int totalCount = 0; // Инициализация переменной для подсчёта общего количества продуктов

                    // Обновляем данные о продаже и сохраняем продукты
                    for (int i = 0; i < selectedProducts.Length; i++)
                    {
                        var product = await _context.Products.FindAsync(selectedProducts[i]);
                        if (product != null)
                        {
                            var salesProduct = new SalesProduct
                            {
                                SalesDataId = salesData.SalesDataId,
                                ProductId = product.ProductId,
                                Quantity = quantities[i]
                            };
                            _context.SalesProducts.Add(salesProduct);

                            // Подсчитываем общую сумму продажи
                            totalAmount += product.Price * quantities[i];

                            // Подсчитываем общее количество продуктов
                            totalCount += quantities[i];
                        }
                    }

                    // Обновляем сумму продажи и количество продуктов
                    salesData.Amount = totalAmount;
                    salesData.SalesCount = totalCount;

                    var totalSalesForMonth = await _context.SalesDatas
                                                    .Where(sd => sd.SaleDate.Year == salesData.MonthStart.Year && sd.SaleDate.Month == salesData.MonthStart.Month)
                                                    .SumAsync(sd => sd.Amount);

                    salesData.SalesPercentage = (totalSalesForMonth / salesData.SalesGoal) * 100;

                    // Сохраняем изменения
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesDataExists(salesData.SalesDataId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Если ModelState недопустим, возвращаем представление с текущей моделью
            return View(salesData);
        }


        private bool SalesDataExists(int id)
        {
            return _context.SalesDatas.Any(e => e.SalesDataId == id);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var salesData = await _context.SalesDatas.FindAsync(id);

            if (salesData == null)
            {
                return NotFound();
            }

            _context.SalesDatas.Remove(salesData);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        


    }
}
