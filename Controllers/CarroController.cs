using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly CarroDAO _carroDAO;
        private readonly ClienteDAO _clienteDAO;

        public CarroController(IConfiguration config)
        {
            _carroDAO = new CarroDAO(config);
            _clienteDAO = new ClienteDAO(config);
        }

        public IActionResult Index()
        {
            var carros = _carroDAO.ListarTodos();
            return View(carros);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            ViewBag.Clientes = _clienteDAO.ListarTodos();
            return View(new CarroViewModel());
        }

        [HttpPost]
        public IActionResult Criar(CarroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _clienteDAO.ListarTodos();
                return View(model);
            }
            _carroDAO.Inserir(model);
            TempData["Sucesso"] = "Veículo cadastrado com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var carro = _carroDAO.BuscarPorId(id);
            if (carro == null) return NotFound();
            ViewBag.Clientes = _clienteDAO.ListarTodos();
            return View(carro);
        }

        [HttpPost]
        public IActionResult Editar(CarroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = _clienteDAO.ListarTodos();
                return View(model);
            }
            _carroDAO.Atualizar(model);
            TempData["Sucesso"] = "Veículo atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            try
            {
                _carroDAO.Excluir(id);
                TempData["Sucesso"] = "Veículo excluído com sucesso!";
            }
            catch (SqlException)
            {
                TempData["Erro"] = "Não é possível excluir este veículo pois ele possui ordens de serviço vinculadas.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult ListarPorCliente(int clienteId)
        {
            var carros = _carroDAO.ListarPorCliente(clienteId)
                .Select(c => new { value = c.Id, text = $"{c.Modelo} - {c.Placa}" });
            return Json(carros);
        }
    }
}
