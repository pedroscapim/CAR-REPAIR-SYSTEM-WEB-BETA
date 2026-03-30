using Microsoft.Data.SqlClient;
using OficinaWeb.Models;

namespace OficinaWeb.DAL
{
    public class LoginDAO
    {
        private readonly string _connectionString;

        public LoginDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public (int Id, string Nome)? Autenticar(string username, string senha)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT IDLOGIN, NOME FROM LOGIN_SISTEMA WHERE USERNAME = @USERNAME AND SENHA = @SENHA", con);
            cmd.Parameters.AddWithValue("@USERNAME", username);
            cmd.Parameters.AddWithValue("@SENHA", senha);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return (reader.GetInt32(0), reader.GetString(1));
            return null;
        }

        public int Inserir(CadastroLoginViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO LOGIN_SISTEMA (NOME, USERNAME, SENHA) VALUES (@NOME, @USERNAME, @SENHA)", con);
            cmd.Parameters.AddWithValue("@NOME", obj.Nome);
            cmd.Parameters.AddWithValue("@USERNAME", obj.Username);
            cmd.Parameters.AddWithValue("@SENHA", obj.Senha);
            return cmd.ExecuteNonQuery();
        }

        public List<UsuarioListaViewModel> ListarTodos()
        {
            var lista = new List<UsuarioListaViewModel>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT IDLOGIN, NOME, USERNAME FROM LOGIN_SISTEMA ORDER BY NOME", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new UsuarioListaViewModel
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Username = reader.GetString(2),
                });
            return lista;
        }

        public int Atualizar(AlterarUsuarioViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            SqlCommand cmd;
            if (!string.IsNullOrWhiteSpace(obj.NovaSenha))
            {
                cmd = new SqlCommand(
                    "UPDATE LOGIN_SISTEMA SET NOME=@NOME, USERNAME=@USERNAME, SENHA=@SENHA WHERE IDLOGIN=@ID", con);
                cmd.Parameters.AddWithValue("@SENHA", obj.NovaSenha);
            }
            else
            {
                cmd = new SqlCommand(
                    "UPDATE LOGIN_SISTEMA SET NOME=@NOME, USERNAME=@USERNAME WHERE IDLOGIN=@ID", con);
            }
            cmd.Parameters.AddWithValue("@NOME", obj.Nome);
            cmd.Parameters.AddWithValue("@USERNAME", obj.Username);
            cmd.Parameters.AddWithValue("@ID", obj.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Excluir(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM LOGIN_SISTEMA WHERE IDLOGIN=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
