using System.ComponentModel.DataAnnotations;

namespace OficinaWeb.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = "";
    }

    public class CadastroLoginViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = "";
    }

    public class UsuarioListaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Username { get; set; } = "";
    }

    public class AlterarUsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Username { get; set; } = "";

        // Se vazio, mantém a senha atual
        [DataType(DataType.Password)]
        public string? NovaSenha { get; set; }
    }
}
