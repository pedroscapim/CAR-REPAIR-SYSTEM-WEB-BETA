using System.ComponentModel.DataAnnotations;

namespace OficinaWeb.Models
{
    public class OrcamentoViewModel
    {
        public int OsId { get; set; }

        [Required(ErrorMessage = "Selecione um cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Selecione um veículo")]
        public int CarroId { get; set; }

        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime DataAbertura { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "KM atual é obrigatório")]
        public string KmAtual { get; set; } = "";

        public List<PecaItemViewModel> Pecas { get; set; } = new();
        public List<ServicoItemViewModel> Servicos { get; set; } = new();

        public decimal TotalPecas { get; set; }
        public decimal TotalServicos { get; set; }
        public decimal Total => TotalPecas + TotalServicos;
    }

    public class PecaItemViewModel
    {
        public string Nome { get; set; } = "";
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }

    public class ServicoItemViewModel
    {
        public string Descricao { get; set; } = "";
        public decimal Valor { get; set; }
        public string TempoGasto { get; set; } = "";
    }
}
