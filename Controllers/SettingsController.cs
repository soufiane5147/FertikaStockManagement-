using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        // Réservé à l'Administrator
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            return View();
        }

        // Accessible par tous les utilisateurs connectés
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        // Accessible par tous les utilisateurs connectés
        [HttpGet]
        public IActionResult Security()
        {
            return View();
        }
    }
}