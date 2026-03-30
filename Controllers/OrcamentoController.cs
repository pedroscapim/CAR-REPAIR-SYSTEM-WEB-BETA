using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class OrcamentoController : Controller
    {
        private readonly OrdemServicoDAO _osDAO;
        private readonly ClienteDAO _clienteDAO;

        public OrcamentoController(IConfiguration config)
        {
            _osDAO = new OrdemServicoDAO(config);
            _clienteDAO = new ClienteDAO(config);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Clientes = _clienteDAO.ListarTodos();
            return View(new OrcamentoViewModel());
        }

        [HttpPost]
        public IActionResult Finalizar([FromBody] OrcamentoViewModel model)
        {
            if (model.ClienteId == 0 || model.CarroId == 0)
                return Json(new { sucesso = false, mensagem = "Selecione cliente e veículo." });

            if (string.IsNullOrWhiteSpace(model.KmAtual))
                return Json(new { sucesso = false, mensagem = "Informe a KM atual." });

            int osId = _osDAO.InserirOS(model.ClienteId, model.CarroId, model.DataAbertura, model.KmAtual);

            if (osId <= 0)
                return Json(new { sucesso = false, mensagem = "Erro ao criar Ordem de Serviço." });

            foreach (var peca in model.Pecas)
                _osDAO.InserirPeca(osId, peca);

            foreach (var servico in model.Servicos)
                _osDAO.InserirServico(osId, servico);

            _osDAO.AtualizarTotais(osId, model.TotalPecas, model.TotalServicos, model.Total);

            return Json(new { sucesso = true, osId, mensagem = $"Ordem de Serviço nº {osId} gerada com sucesso!" });
        }
    }
}
