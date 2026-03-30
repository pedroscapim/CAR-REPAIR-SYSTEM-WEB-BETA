using System.ComponentModel.DataAnnotations;

namespace OficinaWeb.Models
{
    public class CarroViewModel
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Modelo é obrigatório")]
        public string Modelo { get; set; } = "";

        [Required(ErrorMessage = "Placa é obrigatória")]
        public string Placa { get; set; } = "";

        public string AnoFabricacao { get; set; } = "";
        public string Cor { get; set; } = "";
        public string Km { get; set; } = "";

        public string NomeCliente { get; set; } = "";
    }
}
