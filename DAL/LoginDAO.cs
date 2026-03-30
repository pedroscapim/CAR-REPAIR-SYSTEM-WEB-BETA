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
    }
}
