namespace OficinaWeb.Models
{
    public class OSImpressaoViewModel
    {
        // Dados da OS
        public int OsId { get; set; }
        public string NomeCliente { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Modelo { get; set; } = "";
        public string Placa { get; set; } = "";
        public string DataAbertura { get; set; } = "";
        public string KmAtual { get; set; } = "";
        public decimal TotalPecas { get; set; }
        public decimal TotalServicos { get; set; }
        public decimal Total { get; set; }

        public List<PecaImpressao> Pecas { get; set; } = new();
        public List<ServicoImpressao> Servicos { get; set; } = new();

        // Dados da oficina (cabeçalho)
        public string NomeFantasia { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Telefone1 { get; set; } = "";
        public string LogoUrl { get; set; } = "";

        public class PecaImpressao
        {
            public string Nome { get; set; } = "";
            public int Quantidade { get; set; }
            public decimal ValorUnitario { get; set; }
            public decimal ValorTotal { get; set; }
        }

        public class ServicoImpressao
        {
            public string Descricao { get; set; } = "";
            public string TempoGasto { get; set; } = "";
            public decimal Valor { get; set; }
        }
    }
}
