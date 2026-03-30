using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ClienteDAO _dao;

        public ClienteController(IConfiguration config)
        {
            _dao = new ClienteDAO(config);
        }

        public IActionResult Index()
        {
            var clientes = _dao.ListarTodos();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Criar() => View(new ClienteViewModel());

        [HttpPost]
        public IActionResult Criar(ClienteViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _dao.Inserir(model);
            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var cliente = _dao.BuscarPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Editar(ClienteViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _dao.Atualizar(model);
                TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            try
            {
                _dao.Excluir(id);
                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }
            catch (SqlException)
            {
                TempData["Erro"] = "Não é possível excluir este cliente pois ele possui veículos ou ordens de serviço vinculadas.";
            }
            return RedirectToAction("Index");
        }
    }
}
