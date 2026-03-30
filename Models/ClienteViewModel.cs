using System.ComponentModel.DataAnnotations;

namespace OficinaWeb.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Telefone { get; set; } = "";

        public string Endereco { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cep { get; set; } = "";
        public string NumeroCasa { get; set; } = "";
        public string Complemento { get; set; } = "";
    }
}
