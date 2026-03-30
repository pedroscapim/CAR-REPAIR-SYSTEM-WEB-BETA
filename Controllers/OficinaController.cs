using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class OficinaController : Controller
    {
        private readonly OficinaDAO _dao;

        public OficinaController(IConfiguration config)
        {
            _dao = new OficinaDAO(config);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var oficina = _dao.Carregar() ?? new OficinaViewModel();
            return View(oficina);
        }

        [HttpPost]
        public IActionResult Index(OficinaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_dao.Existe())
                _dao.Atualizar(model);
            else
                _dao.Inserir(model);

            TempData["Sucesso"] = "Dados da oficina salvos com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
