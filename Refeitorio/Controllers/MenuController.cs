using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refeitorio.Extensions;
using Refeitorio.Models;
using Refeitorio.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Refeitorio.Controllers
{
    public class MenuController : Controller
    {
        private const string SessionBookingsKey = "UserBookings";

        public IActionResult Index(string option = "Normal", int week = 1)
        {
            // carregar dados
            var allWeeks = MenuRepository.GetAllWeeks();

            // obter semanas disponíveis para abas
            var weeksAvailable = allWeeks.Select(w => w.WeekNumber).ToList();

            // tentar parse do option
            Enum.TryParse<MenuOption>(option, true, out var parsedOption);

            // selecionar a semana pedida
            var weekModel = allWeeks.FirstOrDefault(w => w.WeekNumber == week) ?? allWeeks.First();

            // filtrar apenas os dias da opção selecionada
            var filteredWeek = new MenuWeek
            {
                Id = weekModel.Id,
                WeekNumber = weekModel.WeekNumber,
                MenuDays = weekModel.MenuDays.Where(d => d.Option == parsedOption).ToList()
            };

            // obter marcações do utilizador (da sessão)
            var bookings = HttpContext.Session.GetObject<List<Booking>>(SessionBookingsKey) ?? new List<Booking>();
            ViewBag.Bookings = bookings;
            ViewBag.SelectedOption = parsedOption;
            ViewBag.Weeks = weeksAvailable;
            ViewBag.SelectedWeek = filteredWeek.WeekNumber;

            return View(filteredWeek);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(int dayId, int weekNumber, MenuOption option)
        {
            // simples verificação: já existe marcação para esse dia/semana na sessão?
            var bookings = HttpContext.Session.GetObject<List<Booking>>(SessionBookingsKey) ?? new List<Booking>();

            if (bookings.Any(b => b.DayId == dayId))
            {
                TempData["Message"] = "Já marcou este almoço.";
            }
            else
            {
                bookings.Add(new Booking { DayId = dayId, WeekNumber = weekNumber, Option = option });
                HttpContext.Session.SetObject(SessionBookingsKey, bookings);
                TempData["Message"] = "Almoço marcado com sucesso.";
            }

            // redirecionar de volta com query params para manter a seleção
            return RedirectToAction("Index", new { option = option.ToString(), week = weekNumber });
        }
    }
}