using Microsoft.Data.SqlClient;
using OficinaWeb.Models;

namespace OficinaWeb.DAL
{
    public class ClienteDAO
    {
        private readonly string _connectionString;

        public ClienteDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public int Inserir(ClienteViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO CLIENTE (NOME, TELEFONE, ENDERECO, BAIRRO, CEP, NUMEROCASA, COMPLEMENTO) " +
                "VALUES (@NOME, @TELEFONE, @ENDERECO, @BAIRRO, @CEP, @NUMEROCASA, @COMPLEMENTO)", con);
            BindParams(cmd, obj);
            return cmd.ExecuteNonQuery();
        }

        public int Atualizar(ClienteViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "UPDATE CLIENTE SET NOME=@NOME, TELEFONE=@TELEFONE, ENDERECO=@ENDERECO, " +
                "BAIRRO=@BAIRRO, CEP=@CEP, NUMEROCASA=@NUMEROCASA, COMPLEMENTO=@COMPLEMENTO " +
                "WHERE CLIENTEID=@ID", con);
            BindParams(cmd, obj);
            cmd.Parameters.AddWithValue("@ID", obj.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Excluir(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM CLIENTE WHERE CLIENTEID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);
            return cmd.ExecuteNonQuery();
        }

        public List<ClienteViewModel> ListarTodos()
        {
            var lista = new List<ClienteViewModel>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT CLIENTEID, NOME, TELEFONE, ENDERECO, BAIRRO, CEP, NUMEROCASA, COMPLEMENTO FROM CLIENTE ORDER BY NOME", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Map(reader));
            return lista;
        }

        public ClienteViewModel? BuscarPorId(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT CLIENTEID, NOME, TELEFONE, ENDERECO, BAIRRO, CEP, NUMEROCASA, COMPLEMENTO FROM CLIENTE WHERE CLIENTEID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        private static void BindParams(SqlCommand cmd, ClienteViewModel obj)
        {
            cmd.Parameters.AddWithValue("@NOME", obj.Nome);
            cmd.Parameters.AddWithValue("@TELEFONE", obj.Telefone);
            cmd.Parameters.AddWithValue("@ENDERECO", obj.Endereco ?? "");
            cmd.Parameters.AddWithValue("@BAIRRO", obj.Bairro ?? "");
            cmd.Parameters.AddWithValue("@CEP", obj.Cep ?? "");
            cmd.Parameters.AddWithValue("@NUMEROCASA", obj.NumeroCasa ?? "");
            cmd.Parameters.AddWithValue("@COMPLEMENTO", obj.Complemento ?? "");
        }

        private static ClienteViewModel Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Nome = r.GetString(1),
            Telefone = r.IsDBNull(2) ? "" : r.GetString(2),
            Endereco = r.IsDBNull(3) ? "" : r.GetString(3),
            Bairro = r.IsDBNull(4) ? "" : r.GetString(4),
            Cep = r.IsDBNull(5) ? "" : r.GetString(5),
            NumeroCasa = r.IsDBNull(6) ? "" : r.GetString(6),
            Complemento = r.IsDBNull(7) ? "" : r.GetString(7),
        };
    }
}
