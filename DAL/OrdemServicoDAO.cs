using Microsoft.Data.SqlClient;
using OficinaWeb.Models;
using System.Data;

namespace OficinaWeb.DAL
{
    public class OrdemServicoDAO
    {
        private readonly string _connectionString;

        public OrdemServicoDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public int InserirOS(int clienteId, int carroId, DateTime dataAbertura, string kmAtual)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO ORDEMSERVICO (CLIENTEID, CARROID, DATABERTURA, KMATUAL) " +
                "VALUES (@CLIENTEID, @CARROID, @DATABERTURA, @KMATUAL); " +
                "SELECT SCOPE_IDENTITY();", con);
            cmd.Parameters.AddWithValue("@CLIENTEID", clienteId);
            cmd.Parameters.AddWithValue("@CARROID", carroId);
            cmd.Parameters.AddWithValue("@DATABERTURA", dataAbertura);
            cmd.Parameters.AddWithValue("@KMATUAL", kmAtual);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AtualizarTotais(int osId, decimal totalPecas, decimal totalServicos, decimal total)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "UPDATE ORDEMSERVICO SET PRECOPECAS = @PECAS, PRECOMAODEOBRA = @SERVICOS, PRECOTOTAL = @TOTAL WHERE OSID = @OSID", con);
            cmd.Parameters.AddWithValue("@PECAS", totalPecas);
            cmd.Parameters.AddWithValue("@SERVICOS", totalServicos);
            cmd.Parameters.AddWithValue("@TOTAL", total);
            cmd.Parameters.AddWithValue("@OSID", osId);
            cmd.ExecuteNonQuery();
        }

        public void InserirPeca(int osId, PecaItemViewModel peca)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO PECA (OSID, NOME, QUANTIDADE, VALORUNITARIO, VALORTOTAL) " +
                "VALUES (@OSID, @NOME, @QTD, @VUNI, @VTOTAL)", con);
            cmd.Parameters.AddWithValue("@OSID", osId);
            cmd.Parameters.AddWithValue("@NOME", peca.Nome);
            cmd.Parameters.AddWithValue("@QTD", peca.Quantidade);
            cmd.Parameters.AddWithValue("@VUNI", peca.ValorUnitario);
            cmd.Parameters.AddWithValue("@VTOTAL", peca.ValorTotal);
            cmd.ExecuteNonQuery();
        }

        public void InserirServico(int osId, ServicoItemViewModel servico)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "INSERT INTO SERVICO (OSID, DESCRICAO, VALOR, TEMPOGASTO) " +
                "VALUES (@OSID, @DESCRICAO, @VALOR, @TEMPO)", con);
            cmd.Parameters.AddWithValue("@OSID", osId);
            cmd.Parameters.AddWithValue("@DESCRICAO", servico.Descricao);
            cmd.Parameters.AddWithValue("@VALOR", servico.Valor);
            cmd.Parameters.AddWithValue("@TEMPO", servico.TempoGasto);
            cmd.ExecuteNonQuery();
        }

        public DataTable CarregarDetalheOS(int osId)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(@"
                SELECT DISTINCT
                    OS.OSID,
                    C.NOME AS NomeCliente,
                    C.TELEFONE,
                    V.MODELO,
                    V.PLACA,
                    OS.DATABERTURA,
                    OS.KMATUAL,
                    OS.PRECOPECAS,
                    OS.PRECOMAODEOBRA,
                    OS.PRECOTOTAL,
                    S.DESCRICAO AS ServicoDescricao,
                    S.TEMPOGASTO,
                    S.VALOR AS ValorServico,
                    P.NOME AS NomePeca,
                    P.QUANTIDADE,
                    P.VALORUNITARIO,
                    (P.QUANTIDADE * P.VALORUNITARIO) AS ValorTotalPeca
                FROM ORDEMSERVICO OS
                JOIN CLIENTE C ON C.CLIENTEID = OS.CLIENTEID
                JOIN VEICULO V ON V.CARROID = OS.CARROID
                LEFT JOIN SERVICO S ON S.OSID = OS.OSID
                LEFT JOIN PECA P ON P.OSID = OS.OSID
                WHERE OS.OSID = @OSID
                ORDER BY OS.OSID, S.DESCRICAO, P.NOME", con);
            cmd.Parameters.AddWithValue("@OSID", osId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public void DeletarOS(int osId)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            using var trans = con.BeginTransaction();
            try
            {
                var cmdS = new SqlCommand("DELETE FROM SERVICO WHERE OSID = @OSID", con, trans);
                cmdS.Parameters.AddWithValue("@OSID", osId);
                cmdS.ExecuteNonQuery();

                var cmdP = new SqlCommand("DELETE FROM PECA WHERE OSID = @OSID", con, trans);
                cmdP.Parameters.AddWithValue("@OSID", osId);
                cmdP.ExecuteNonQuery();

                var cmdOS = new SqlCommand("DELETE FROM ORDEMSERVICO WHERE OSID = @OSID", con, trans);
                cmdOS.Parameters.AddWithValue("@OSID", osId);
                cmdOS.ExecuteNonQuery();

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public List<dynamic> ListarOSPorClienteECarro(int clienteId, int carroId)
        {
            var lista = new List<dynamic>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(
                "SELECT OSID, DATABERTURA, KMATUAL FROM ORDEMSERVICO " +
                "WHERE CLIENTEID = @CLIENTEID AND CARROID = @CARROID ORDER BY DATABERTURA DESC", con);
            cmd.Parameters.AddWithValue("@CLIENTEID", clienteId);
            cmd.Parameters.AddWithValue("@CARROID", carroId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new
                {
                    OsId = reader.GetInt32(0),
                    DataAbertura = reader.GetDateTime(1),
                    KmAtual = reader.IsDBNull(2) ? "" : reader.GetString(2),
                });
            }
            return lista;
        }
    }
}
