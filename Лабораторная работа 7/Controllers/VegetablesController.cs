using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Лабораторная_работа_7.Models;

namespace Лабораторная_работа_7.Controllers
{
    public class VegetablesController : Controller
    {
        public ActionResult FirstViewMethod()
        {
            // Создаем список овощей
            List<Vegetable> veggies = new List<Vegetable>
            {
                new Vegetable { Id = 1, Name = "Картофель" },
                new Vegetable { Id = 2, Name = "Морковь" },
                new Vegetable { Id = 3, Name = "Лук" },
                new Vegetable { Id = 4, Name = "Капуста" },
                new Vegetable { Id = 5, Name = "Свекла" },
                new Vegetable { Id = 6, Name = "Баклажан" }
            };
            var sorted = veggies.OrderBy(v => v.Name).ToList();

            // Передаем список в представление в качестве модели
            return View(sorted);
        }

        public ActionResult SecondViewMethod()
        {
            // Создаем список овощей
            List<Vegetable> veggies = new List<Vegetable>
            {
                new Vegetable { Id = 1, Name = "Картофель" },
                new Vegetable { Id = 2, Name = "Морковь" },
                new Vegetable { Id = 3, Name = "Лук" },
                new Vegetable { Id = 4, Name = "Капуста" },
                new Vegetable { Id = 5, Name = "Свекла" },
                new Vegetable { Id = 6, Name = "Баклажан" }
            };
            // var sorted = veggies.OrderBy(v => v.Name).ToList();
            List<Vegetable>? sorted= null;
          
            Dictionary<char?, List<Vegetable>> grouped = new();
            
            char? curr = null;

            //if (sorted == null) return View(null);
            foreach (var vegetable in sorted)
            {
                if (vegetable.Name.First() != curr)
                    curr = vegetable.Name.First();
                if (!grouped.ContainsKey(curr))
                    grouped.Add(curr, new List<Vegetable>());
                grouped[curr].Add(vegetable);
            }
            // Передаем список в представление в качестве модели
            return View(grouped);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
