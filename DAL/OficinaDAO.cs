using Microsoft.Data.SqlClient;
using OficinaWeb.Models;

namespace OficinaWeb.DAL
{
    public class OficinaDAO
    {
        private readonly string _connectionString;

        public OficinaDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public OficinaViewModel? Carregar()
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT TOP 1 IDOFICINA, NOMEFANTASIA, RAZAOSOCIAL, TELEFONE1, TELEFONE2, CEP, ENDERECO, BAIRRO, NUMERO, CIDADE FROM OFICINA", con);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new OficinaViewModel
                {
                    Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    NomeFantasia = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    RazaoSocial = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Telefone1 = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Telefone2 = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Cep = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Endereco = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Bairro = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Numero = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                    Cidade = reader.IsDBNull(9) ? "" : reader.GetString(9),
                };
            }
            return null;
        }

        public bool Existe()
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM OFICINA", con);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int Inserir(OficinaViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO OFICINA (NOMEFANTASIA, RAZAOSOCIAL, TELEFONE1, TELEFONE2, CEP, ENDERECO, BAIRRO, NUMERO, CIDADE) " +
                "VALUES (@NF, @RS, @T1, @T2, @CEP, @END, @BAI, @NUM, @CID)", con);
            cmd.Parameters.AddWithValue("@NF", obj.NomeFantasia);
            cmd.Parameters.AddWithValue("@RS", obj.RazaoSocial ?? "");
            cmd.Parameters.AddWithValue("@T1", obj.Telefone1 ?? "");
            cmd.Parameters.AddWithValue("@T2", obj.Telefone2 ?? "");
            cmd.Parameters.AddWithValue("@CEP", obj.Cep ?? "");
            cmd.Parameters.AddWithValue("@END", obj.Endereco ?? "");
            cmd.Parameters.AddWithValue("@BAI", obj.Bairro ?? "");
            cmd.Parameters.AddWithValue("@NUM", obj.Numero);
            cmd.Parameters.AddWithValue("@CID", obj.Cidade ?? "");
            return cmd.ExecuteNonQuery();
        }

        public int Atualizar(OficinaViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "UPDATE OFICINA SET NOMEFANTASIA=@NF, RAZAOSOCIAL=@RS, TELEFONE1=@T1, TELEFONE2=@T2, " +
                "CEP=@CEP, ENDERECO=@END, BAIRRO=@BAI, NUMERO=@NUM, CIDADE=@CID", con);
            cmd.Parameters.AddWithValue("@NF", obj.NomeFantasia);
            cmd.Parameters.AddWithValue("@RS", obj.RazaoSocial ?? "");
            cmd.Parameters.AddWithValue("@T1", obj.Telefone1 ?? "");
            cmd.Parameters.AddWithValue("@T2", obj.Telefone2 ?? "");
            cmd.Parameters.AddWithValue("@CEP", obj.Cep ?? "");
            cmd.Parameters.AddWithValue("@END", obj.Endereco ?? "");
            cmd.Parameters.AddWithValue("@BAI", obj.Bairro ?? "");
            cmd.Parameters.AddWithValue("@NUM", obj.Numero);
            cmd.Parameters.AddWithValue("@CID", obj.Cidade ?? "");
            return cmd.ExecuteNonQuery();
        }
    }
}
