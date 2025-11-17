using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refeitorio.Extensions;
using Refeitorio.Models;
using Refeitorio.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Refeitorio.Controllers
{
    public class MenuController : Controller
    {
        private const string SessionBookingsKey = "UserBookings";

        // option: "Normal", "Vegetariano" ou "All"
        public IActionResult Index(string option = "Normal", int week = 1)
        {
            // carregar dados
            var allWeeks = MenuRepository.GetAllWeeks();

            // obter semanas disponíveis para abas
            var weeksAvailable = allWeeks.Select(w => w.WeekNumber).ToList();

            // construir intervalo de datas por semana para exibição (ex: 10/11/2025 - 14/11/2025)
            var weekRanges = new Dictionary<int, string>();
            //foreach (var w in allWeeks)
            //{
            //    var minDate = w.MenuDays.Min(d => d.Date);
            //    var start = minDate.Date;
            //    var end = start.AddDays(4);
            //    weekRanges[w.WeekNumber] = $"{start:dd/MM/yyyy} - {end:dd/MM/yyyy}";
            //}

            // tratar "All"
            var showAll = string.Equals(option, "All", StringComparison.OrdinalIgnoreCase);

            // tentar parse do option apenas se não for "All"
            MenuOption parsedOption = MenuOption.Normal;
            if (!showAll)
            {
                Enum.TryParse<MenuOption>(option, true, out parsedOption);
            }

            // selecionar a semana pedida
            var weekModel = allWeeks.FirstOrDefault(w => w.WeekNumber == week) ?? allWeeks.First();

            // filtrar dias de acordo com a opção (ou mostrar todos se "All")
            var filteredDays = showAll
                ? weekModel.MenuDays.ToList()
                : weekModel.MenuDays.Where(d => d.Option == parsedOption).ToList();

            var filteredWeek = new MenuWeek
            {
                Id = weekModel.Id,
                WeekNumber = weekModel.WeekNumber,
                MenuDays = filteredDays
            };

            // obter marcações do utilizador (da sessão)
            var bookings = HttpContext.Session.GetObject<List<Booking>>(SessionBookingsKey) ?? new List<Booking>();
            ViewBag.Bookings = bookings;
            ViewBag.SelectedOption = option; // string: "Normal", "Vegetariano" ou "All"
            ViewBag.Weeks = weeksAvailable;
            ViewBag.SelectedWeek = filteredWeek.WeekNumber;
            ViewBag.WeekRanges = weekRanges;
            ViewBag.CurrentWeekRange = weekRanges.ContainsKey(filteredWeek.WeekNumber) ? weekRanges[filteredWeek.WeekNumber] : string.Empty;

            return View(filteredWeek);
        }

        // Adicionámos returnOption para preservar a opção original ("All" por exemplo) ao redirecionar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(int dayId, int weekNumber, MenuOption option, string returnOption)
        {
            var bookings = HttpContext.Session.GetObject<List<Booking>>(SessionBookingsKey) ?? new List<Booking>();

            // carregar todos os weeks para mapear ids -> datas
            var allWeeks = MenuRepository.GetAllWeeks();

            // procurar a MenuDay correspondente para obter a data real
            var menuDay = allWeeks
                .FirstOrDefault(w => w.WeekNumber == weekNumber)?
                .MenuDays.FirstOrDefault(d => d.Id == dayId);

            if (menuDay == null)
            {
                TempData["Message"] = "Dia inválido.";
                return RedirectToAction("Index", new { option = returnOption ?? option.ToString(), week = weekNumber });
            }

            // Verificação: já existe marcação para ESTE dia (mesma data) — impedimos marcar Normal+Vegetariano no mesmo dia
            //var existingBookingDates = bookings
            //    .Select(b => allWeeks.FirstOrDefault(w => w.WeekNumber == b.WeekNumber)?
            //                     .MenuDays.FirstOrDefault(d => d.Id == b.DayId)?.Date)
            //    .Where(d => d.HasValue)
            //    .Select(d => d!.Value);

            //if (BookingRules.HasConflictByDate(menuDay.Date, existingBookingDates))
            //{
            //    TempData["Message"] = "Já tem uma marcação para este dia. Não pode marcar outra opção no mesmo dia.";
            //    return RedirectToAction("Index", new { option = returnOption ?? option.ToString(), week = weekNumber });
            //}

            // simples verificação: já existe marcação para esse dayId/semana na sessão?
            if (bookings.Any(b => b.DayId == dayId && b.WeekNumber == weekNumber))
            {
                TempData["Message"] = "Já marcou este almoço.";
                return RedirectToAction("Index", new { option = returnOption ?? option.ToString(), week = weekNumber });
            }

            // usar as regras de reserva (prazo)
            //if (!BookingRules.CanBook(menuDay.Date))
            //{
            //    TempData["Message"] = "Não é possível marcar este almoço: prazo de marcação expirado.";
            //    return RedirectToAction("Index", new { option = returnOption ?? option.ToString(), week = weekNumber });
            //}

            bookings.Add(new Booking { DayId = dayId, WeekNumber = weekNumber, Option = option });
            HttpContext.Session.SetObject(SessionBookingsKey, bookings);
            TempData["Message"] = "Almoço marcado com sucesso.";

            // redirecionar de volta com query params para manter a seleção (usa returnOption para preservar "All")
            return RedirectToAction("Index", new { option = returnOption ?? option.ToString(), week = weekNumber });
        }
    }
}