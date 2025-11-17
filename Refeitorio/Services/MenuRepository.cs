using Refeitorio.Models;
using System.Collections.Generic;
using System;

namespace Refeitorio.Services
{
    // Repositório em memória com menus de exemplo (Semana 1 e Semana 2)
    public static class MenuRepository
    {
        // Segunda-feira base para a Semana 1 (conforme exemplo pedido)
        // Semana 1 -> 10/11/2025 - 14/11/2025
        private static readonly DateTime BaseMonday = new DateTime(2025, 11, 10);

        public static List<MenuWeek> GetAllWeeks()
        {
            // Exemplo simples: para cada semana, cada dia tem duas opções (Normal + Vegetariano)
            var weeks = new List<MenuWeek>();

            // Semana 1
            var week1 = new MenuWeek { Id = 1, WeekNumber = 1 };
            var week1Start = BaseMonday.AddDays((week1.WeekNumber - 1) * 7);
            //week1.MenuDays.AddRange(new[]
            //{
            //    new MenuDay { Id = 101, Weekday = "Segunda", Option = MenuOption.Normal, MainDish = "Bife com Batatas Fritas", Soup = "Canja de Galinha", Dessert = "Mousse de Chocolate", Date = week1Start.AddDays(0) },
            //    new MenuDay { Id = 102, Weekday = "Segunda", Option = MenuOption.Vegetariano, MainDish = "Hambúrguer Vegetal com Batatas", Soup = "Creme de Legumes", Dessert = "Fruta", Date = week1Start.AddDays(0) },

            //    new MenuDay { Id = 103, Weekday = "Terça", Option = MenuOption.Normal, MainDish = "Bacalhau à Brás", Soup = "Sopa de Legumes", Dessert = "Arroz Doce", Date = week1Start.AddDays(1) },
            //    new MenuDay { Id = 104, Weekday = "Terça", Option = MenuOption.Vegetariano, MainDish = "Tofu Grelhado com Arroz", Soup = "Sopa de Legumes", Dessert = "Fruta", Date = week1Start.AddDays(1) },

            //    new MenuDay { Id = 105, Weekday = "Quarta", Option = MenuOption.Normal, MainDish = "Frango Assado", Soup = "Sopa de Cenoura", Dessert = "Gelatina", Date = week1Start.AddDays(2) },
            //    new MenuDay { Id = 106, Weekday = "Quarta", Option = MenuOption.Vegetariano, MainDish = "Lasanha de Legumes", Soup = "Sopa de Cenoura", Dessert = "Gelatina", Date = week1Start.AddDays(2) },

            //    new MenuDay { Id = 107, Weekday = "Quinta", Option = MenuOption.Normal, MainDish = "Carne de Porco Estufada", Soup = "Sopa de Feijão", Dessert = "Pudim", Date = week1Start.AddDays(3) },
            //    new MenuDay { Id = 108, Weekday = "Quinta", Option = MenuOption.Vegetariano, MainDish = "Curry de Grão-de-Bico", Soup = "Sopa de Feijão", Dessert = "Pudim", Date = week1Start.AddDays(3) },

            //    new MenuDay { Id = 109, Weekday = "Sexta", Option = MenuOption.Normal, MainDish = "Peixe Grelhado", Soup = "Sopa de Peixe", Dessert = "Doce", Date = week1Start.AddDays(4) },
            //    new MenuDay { Id = 110, Weekday = "Sexta", Option = MenuOption.Vegetariano, MainDish = "Salada Completa", Soup = "Sopa Fria", Dessert = "Doce", Date = week1Start.AddDays(4) },
            //});
            weeks.Add(week1);

            // Semana 2 (exemplo diferente)
            var week2 = new MenuWeek { Id = 2, WeekNumber = 2 };
            var week2Start = BaseMonday.AddDays((week2.WeekNumber - 1) * 7);
            //week2.MenuDays.AddRange(new[]
            //{
            //    new MenuDay { Id = 201, Weekday = "Segunda", Option = MenuOption.Normal, MainDish = "Carne Estufada", Soup = "Sopa de Tomate", Dessert = "Mousse", Date = week2Start.AddDays(0) },
            //    new MenuDay { Id = 202, Weekday = "Segunda", Option = MenuOption.Vegetariano, MainDish = "Esparguete de Legumes", Soup = "Sopa de Tomate", Dessert = "Fruta", Date = week2Start.AddDays(0) },

            //    new MenuDay { Id = 203, Weekday = "Terça", Option = MenuOption.Normal, MainDish = "Arroz de Pato", Soup = "Sopa ", Dessert = "Arroz Doce", Date = week2Start.AddDays(1) },
            //    new MenuDay { Id = 204, Weekday = "Terça", Option = MenuOption.Vegetariano, MainDish = "Arroz de Legumes", Soup = "Sopa", Dessert = "Fruta", Date = week2Start.AddDays(1) },

            //    new MenuDay { Id = 205, Weekday = "Quarta", Option = MenuOption.Normal, MainDish = "Frango Grelhado", Soup = "Sopa de Legumes", Dessert = "Gelatina", Date = week2Start.AddDays(2) },
            //    new MenuDay { Id = 206, Weekday = "Quarta", Option = MenuOption.Vegetariano, MainDish = "Risotto de Cogumelos", Soup = "Sopa de Legumes", Dessert = "Gelatina", Date = week2Start.AddDays(2) },

            //    new MenuDay { Id = 207, Weekday = "Quinta", Option = MenuOption.Normal, MainDish = "Carne no Forno", Soup = "Creme", Dessert = "Pudim", Date = week2Start.AddDays(3) },
            //    new MenuDay { Id = 208, Weekday = "Quinta", Option = MenuOption.Vegetariano, MainDish = "Tacos de Legumes", Soup = "Creme", Dessert = "Pudim", Date = week2Start.AddDays(3) },

            //    new MenuDay { Id = 209, Weekday = "Sexta", Option = MenuOption.Normal, MainDish = "Filete de Peixe", Soup = "Sopa de Peixe", Dessert = "Doce", Date = week2Start.AddDays(4) },
            //    new MenuDay { Id = 210, Weekday = "Sexta", Option = MenuOption.Vegetariano, MainDish = "Quiche de Espinafres", Soup = "Sopa Fria", Dessert = "Doce", Date = week2Start.AddDays(4) },
            //});
            weeks.Add(week2);

            return weeks;
        }
    }
}