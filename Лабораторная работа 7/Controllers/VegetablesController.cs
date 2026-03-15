using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using Лабораторная_работа_7.Models;

namespace Лабораторная_работа_7.Controllers
{
    public class VegetablesController : Controller
    {
        private List<Vegetable> GetVegetables()
        {
            //return null;
            return new List<Vegetable>
            {
                new Vegetable { Id = 1, Name = "Картофель" },
                new Vegetable { Id = 2, Name = "Морковь" },
                new Vegetable { Id = 3, Name = "Лук" },
                new Vegetable { Id = 4, Name = "Капуста" },
                new Vegetable { Id = 5, Name = "Свекла" },
                new Vegetable { Id = 6, Name = "Баклажан" }
            };
        }

        public ActionResult FirstViewMethod()
        {
            var veggies = GetVegetables();
            var sorted = veggies.OrderBy(v => v.Name).ToList();

            if (!sorted.Any())
                return View("EmptyList");

            return View(sorted);
        }

        public ActionResult SecondViewMethod()
        {
            var veggies = GetVegetables();
            var sorted = veggies.OrderBy(v => v.Name).ToList();

            if (!sorted.Any())
                return View("EmptyList");

            Dictionary<char?, List<Vegetable>> grouped = new();

            char? curr = null;

            foreach (var vegetable in sorted)
            {
                if (vegetable.Name.First() != curr)
                    curr = vegetable.Name.First();
                if (!grouped.ContainsKey(curr))
                    grouped.Add(curr, new List<Vegetable>());
                grouped[curr].Add(vegetable);
            }

            return View(grouped);
        }

        public IActionResult EmptyList()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var errorMessage = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error?.Message;
            ViewBag.ErrorMessage = errorMessage;
            return View(new ErrorViewModel { RequestId = errorId });
        }
    }
}
