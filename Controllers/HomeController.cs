using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DashboardDAO _dashDAO;

        public HomeController(IConfiguration config)
        {
            _dashDAO = new DashboardDAO(config);
        }

        public IActionResult Index() => View();

        public IActionResult MenuPrincipal() => View();

        [HttpGet]
        public IActionResult DadosDashboard(DateTime? dataInicio, DateTime? dataFim)
        {
            var inicio = dataInicio ?? new DateTime(DateTime.Today.Year, 1, 1);
            var fim = dataFim ?? DateTime.Today;

            var mensal = _dashDAO.GetDadosMensais(inicio, fim);
            var resumo = _dashDAO.GetResumo(inicio, fim);

            return Json(new
            {
                labels = mensal.Select(x => x.Label).ToList(),
                maoDeObra = mensal.Select(x => x.TotalMaoDeObra).ToList(),
                pecas = mensal.Select(x => x.TotalPecas).ToList(),
                qtdOS = mensal.Select(x => x.QtdOS).ToList(),
                resumo = new
                {
                    resumo.QtdOS,
                    resumo.TotalMaoDeObra,
                    resumo.TotalPecas,
                    resumo.TotalGeral,
                }
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
