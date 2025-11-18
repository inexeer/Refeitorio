using Microsoft.AspNetCore.Mvc.Rendering;
using Refeitorio.Models;

namespace Refeitorio.ViewModels
{
    public class SelectLunchViewModel
    {
        public DateOnly Data { get; set; }
        public int? SelectedVegetarianId { get; set; }
        public int? SelectedNormalId { get; set; }
        public List<SelectListItem> VegOptions { get; set; } = new();
        public List<SelectListItem> NormalOptions { get; set; } = new();
    }
}
