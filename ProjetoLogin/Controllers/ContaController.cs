using Microsoft.AspNetCore.Mvc;

namespace ProjetoLogin.Controllers
{
    public class ContaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
