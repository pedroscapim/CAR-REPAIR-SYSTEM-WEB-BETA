using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class ConsultaController : Controller
    {
        private readonly OrdemServicoDAO _osDAO;
        private readonly ClienteDAO _clienteDAO;
        private readonly CarroDAO _carroDAO;
        private readonly OficinaDAO _oficinaDAO;

        public ConsultaController(IConfiguration config)
        {
            _osDAO = new OrdemServicoDAO(config);
            _clienteDAO = new ClienteDAO(config);
            _carroDAO = new CarroDAO(config);
            _oficinaDAO = new OficinaDAO(config);
        }

        public IActionResult Index()
        {
            ViewBag.Clientes = _clienteDAO.ListarTodos();
            return View();
        }

        [HttpGet]
        public JsonResult ListarOSPorClienteECarro(int clienteId, int carroId)
        {
            var lista = _osDAO.ListarOSPorClienteECarro(clienteId, carroId)
                .Select(x => new
                {
                    value = (int)x.OsId,
                    text = ((DateTime)x.DataAbertura).ToString("dd/MM/yyyy")
                });
            return Json(lista);
        }

        [HttpGet]
        public IActionResult Detalhes(int osId)
        {
            var dt = _osDAO.CarregarDetalheOS(osId);
            if (dt.Rows.Count == 0) return NotFound();

            var pecas = new List<object>();
            var servicos = new List<object>();
            var pecasVistas = new HashSet<string>();
            var servicosVistos = new HashSet<string>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (row["NomePeca"] != DBNull.Value)
                {
                    string key = row["NomePeca"] + "_" + row["QUANTIDADE"] + "_" + row["VALORUNITARIO"];
                    if (pecasVistas.Add(key))
                        pecas.Add(new { nome = row["NomePeca"], qtd = row["QUANTIDADE"], valorUni = row["VALORUNITARIO"], valorTotal = row["ValorTotalPeca"] });
                }
                if (row["ServicoDescricao"] != DBNull.Value)
                {
                    string key = row["ServicoDescricao"] + "_" + row["ValorServico"];
                    if (servicosVistos.Add(key))
                        servicos.Add(new { descricao = row["ServicoDescricao"], tempo = row["TEMPOGASTO"], valor = row["ValorServico"] });
                }
            }

            var row0 = dt.Rows[0];
            return Json(new
            {
                osId,
                cliente = row0["NomeCliente"],
                telefone = row0["TELEFONE"],
                modelo = row0["MODELO"],
                placa = row0["PLACA"],
                dataAbertura = Convert.ToDateTime(row0["DATABERTURA"]).ToString("dd/MM/yyyy"),
                kmAtual = row0["KMATUAL"],
                totalPecas = row0["PRECOPECAS"],
                totalServicos = row0["PRECOMAODEOBRA"],
                total = row0["PRECOTOTAL"],
                pecas,
                servicos,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletarOS(int osId)
        {
            _osDAO.DeletarOS(osId);
            TempData["Sucesso"] = "Ordem de Serviço excluída com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Imprimir(int osId)
        {
            var dt = _osDAO.CarregarDetalheOS(osId);
            if (dt.Rows.Count == 0) return NotFound();

            var vm = new OSImpressaoViewModel();
            var pecasVistas = new HashSet<string>();
            var servicosVistos = new HashSet<string>();

            var row0 = dt.Rows[0];
            vm.OsId = osId;
            vm.NomeCliente = row0["NomeCliente"].ToString()!;
            vm.Telefone = row0["TELEFONE"].ToString()!;
            vm.Modelo = row0["MODELO"].ToString()!;
            vm.Placa = row0["PLACA"].ToString()!;
            vm.DataAbertura = Convert.ToDateTime(row0["DATABERTURA"]).ToString("dd/MM/yyyy");
            vm.KmAtual = row0["KMATUAL"].ToString()!;
            vm.TotalPecas = row0["PRECOPECAS"] == DBNull.Value ? 0 : Convert.ToDecimal(row0["PRECOPECAS"]);
            vm.TotalServicos = row0["PRECOMAODEOBRA"] == DBNull.Value ? 0 : Convert.ToDecimal(row0["PRECOMAODEOBRA"]);
            vm.Total = row0["PRECOTOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(row0["PRECOTOTAL"]);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (row["NomePeca"] != DBNull.Value)
                {
                    string key = row["NomePeca"] + "_" + row["QUANTIDADE"] + "_" + row["VALORUNITARIO"];
                    if (pecasVistas.Add(key))
                        vm.Pecas.Add(new OSImpressaoViewModel.PecaImpressao
                        {
                            Nome = row["NomePeca"].ToString()!,
                            Quantidade = Convert.ToInt32(row["QUANTIDADE"]),
                            ValorUnitario = Convert.ToDecimal(row["VALORUNITARIO"]),
                            ValorTotal = Convert.ToDecimal(row["ValorTotalPeca"]),
                        });
                }
                if (row["ServicoDescricao"] != DBNull.Value)
                {
                    string key = row["ServicoDescricao"] + "_" + row["ValorServico"];
                    if (servicosVistos.Add(key))
                        vm.Servicos.Add(new OSImpressaoViewModel.ServicoImpressao
                        {
                            Descricao = row["ServicoDescricao"].ToString()!,
                            TempoGasto = row["TEMPOGASTO"].ToString()!,
                            Valor = Convert.ToDecimal(row["ValorServico"]),
                        });
                }
            }

            // Dados da oficina para o cabeçalho
            var oficina = _oficinaDAO.Carregar();
            if (oficina != null)
            {
                vm.NomeFantasia = oficina.NomeFantasia;
                vm.Cnpj = oficina.Cnpj;
                vm.Endereco = oficina.Endereco;
                vm.Numero = oficina.Numero.ToString();
                vm.Bairro = oficina.Bairro;
                vm.Cidade = oficina.Cidade;
                vm.Telefone1 = oficina.Telefone1;
            }

            return View(vm);
        }
    }
}
