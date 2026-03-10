using Microsoft.AspNetCore.Mvc;
using Лабораторная_работа_7.Models;

namespace Лабораторная_работа_7.Controllers
{
    public class VegetablesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult FirstViewMethod()
        {
            // Создаем список овощей
            List<Vegetable> veggies = new List<Vegetable>
            {
                new Vegetable { Id = 1, Name = "Картофель" },
                new Vegetable { Id = 2, Name = "Морковь" },
                new Vegetable { Id = 3, Name = "Лук" },
                new Vegetable { Id = 4, Name = "Капуста" },
                new Vegetable { Id = 5, Name = "Свекла" }
            };
            var sorted = veggies.OrderBy(v => v.Name).ToList();
            // Передаем список в представление в качестве модели
            return View(sorted);
        }
    }
}
