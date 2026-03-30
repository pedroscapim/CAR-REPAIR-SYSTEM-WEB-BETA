using Microsoft.Data.SqlClient;
using OficinaWeb.Models;

namespace OficinaWeb.DAL
{
    public class CarroDAO
    {
        private readonly string _connectionString;

        public CarroDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public int Inserir(CarroViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO VEICULO (CLIENTEID, MODELO, PLACA, ANOCARRO, COR, KM) " +
                "VALUES (@CLIENTEID, @MODELO, @PLACA, @ANOCARRO, @COR, @KM)", con);
            BindParams(cmd, obj);
            return cmd.ExecuteNonQuery();
        }

        public int Atualizar(CarroViewModel obj)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "UPDATE VEICULO SET CLIENTEID=@CLIENTEID, MODELO=@MODELO, PLACA=@PLACA, " +
                "ANOCARRO=@ANOCARRO, COR=@COR, KM=@KM WHERE CARROID=@ID", con);
            BindParams(cmd, obj);
            cmd.Parameters.AddWithValue("@ID", obj.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Excluir(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand("DELETE FROM VEICULO WHERE CARROID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);
            return cmd.ExecuteNonQuery();
        }

        public CarroViewModel? BuscarPorId(int id)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT V.CARROID, V.CLIENTEID, V.MODELO, V.PLACA, V.ANOCARRO, V.COR, V.KM, C.NOME " +
                "FROM VEICULO V JOIN CLIENTE C ON C.CLIENTEID = V.CLIENTEID WHERE V.CARROID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public List<CarroViewModel> ListarPorCliente(int clienteId)
        {
            var lista = new List<CarroViewModel>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT CARROID, CLIENTEID, MODELO, PLACA, ANOCARRO, COR, KM FROM VEICULO WHERE CLIENTEID=@CLIENTEID ORDER BY MODELO", con);
            cmd.Parameters.AddWithValue("@CLIENTEID", clienteId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(MapSemCliente(reader));
            return lista;
        }

        public List<CarroViewModel> ListarTodos()
        {
            var lista = new List<CarroViewModel>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT V.CARROID, V.CLIENTEID, V.MODELO, V.PLACA, V.ANOCARRO, V.COR, V.KM, C.NOME " +
                "FROM VEICULO V JOIN CLIENTE C ON C.CLIENTEID = V.CLIENTEID ORDER BY C.NOME, V.MODELO", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Map(reader));
            return lista;
        }

        private static void BindParams(SqlCommand cmd, CarroViewModel obj)
        {
            cmd.Parameters.AddWithValue("@CLIENTEID", obj.ClienteId);
            cmd.Parameters.AddWithValue("@MODELO", obj.Modelo);
            cmd.Parameters.AddWithValue("@PLACA", obj.Placa);
            cmd.Parameters.AddWithValue("@ANOCARRO", obj.AnoFabricacao ?? "");
            cmd.Parameters.AddWithValue("@COR", obj.Cor ?? "");
            cmd.Parameters.AddWithValue("@KM", obj.Km ?? "");
        }

        private static CarroViewModel Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            ClienteId = r.GetInt32(1),
            Modelo = r.IsDBNull(2) ? "" : r.GetString(2),
            Placa = r.IsDBNull(3) ? "" : r.GetString(3),
            AnoFabricacao = r.IsDBNull(4) ? "" : r.GetString(4),
            Cor = r.IsDBNull(5) ? "" : r.GetString(5),
            Km = r.IsDBNull(6) ? "" : r.GetString(6),
            NomeCliente = r.IsDBNull(7) ? "" : r.GetString(7),
        };

        private static CarroViewModel MapSemCliente(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            ClienteId = r.GetInt32(1),
            Modelo = r.IsDBNull(2) ? "" : r.GetString(2),
            Placa = r.IsDBNull(3) ? "" : r.GetString(3),
            AnoFabricacao = r.IsDBNull(4) ? "" : r.GetString(4),
            Cor = r.IsDBNull(5) ? "" : r.GetString(5),
            Km = r.IsDBNull(6) ? "" : r.GetString(6),
        };
    }
}
