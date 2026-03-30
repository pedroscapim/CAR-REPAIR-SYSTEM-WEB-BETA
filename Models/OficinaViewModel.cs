using System.ComponentModel.DataAnnotations;

namespace OficinaWeb.Models
{
    public class OficinaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        public string NomeFantasia { get; set; } = "";

        public string RazaoSocial { get; set; } = "";
        public string Telefone1 { get; set; } = "";
        public string Telefone2 { get; set; } = "";
        public string Cep { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Bairro { get; set; } = "";
        public int Numero { get; set; }
        public string Cidade { get; set; } = "";

        // Apenas para exibição — não vem do banco
        public string LogoUrl { get; set; } = "";
    }
}
