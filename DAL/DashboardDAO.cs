using Microsoft.Data.SqlClient;

namespace OficinaWeb.DAL
{
    public class DashboardDAO
    {
        private readonly string _connectionString;

        public DashboardDAO(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Oficina")!;
        }

        public List<DadosMensais> GetDadosMensais(DateTime dataInicio, DateTime dataFim)
        {
            var lista = new List<DadosMensais>();
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(@"
                SELECT
                    YEAR(DATABERTURA)  AS Ano,
                    MONTH(DATABERTURA) AS Mes,
                    ISNULL(SUM(PRECOMAODEOBRA), 0) AS TotalMaoDeObra,
                    ISNULL(SUM(PRECOPECAS), 0)     AS TotalPecas,
                    COUNT(*)                        AS QtdOS
                FROM ORDEMSERVICO
                WHERE DATABERTURA >= @INICIO
                  AND DATABERTURA <  DATEADD(DAY, 1, @FIM)
                GROUP BY YEAR(DATABERTURA), MONTH(DATABERTURA)
                ORDER BY Ano, Mes", con);
            cmd.Parameters.AddWithValue("@INICIO", dataInicio.Date);
            cmd.Parameters.AddWithValue("@FIM", dataFim.Date);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DadosMensais
                {
                    Ano = reader.GetInt32(0),
                    Mes = reader.GetInt32(1),
                    TotalMaoDeObra = reader.GetDecimal(2),
                    TotalPecas = reader.GetDecimal(3),
                    QtdOS = reader.GetInt32(4),
                });
            }
            return lista;
        }

        public ResumoGeral GetResumo(DateTime dataInicio, DateTime dataFim)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();
            var cmd = new SqlCommand(@"
                SELECT
                    COUNT(*)                        AS QtdOS,
                    ISNULL(SUM(PRECOMAODEOBRA), 0) AS TotalMaoDeObra,
                    ISNULL(SUM(PRECOPECAS), 0)     AS TotalPecas,
                    ISNULL(SUM(PRECOTOTAL), 0)     AS TotalGeral
                FROM ORDEMSERVICO
                WHERE DATABERTURA >= @INICIO
                  AND DATABERTURA <  DATEADD(DAY, 1, @FIM)", con);
            cmd.Parameters.AddWithValue("@INICIO", dataInicio.Date);
            cmd.Parameters.AddWithValue("@FIM", dataFim.Date);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new ResumoGeral
                {
                    QtdOS = reader.GetInt32(0),
                    TotalMaoDeObra = reader.GetDecimal(1),
                    TotalPecas = reader.GetDecimal(2),
                    TotalGeral = reader.GetDecimal(3),
                };
            }
            return new ResumoGeral();
        }
    }

    public class DadosMensais
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal TotalMaoDeObra { get; set; }
        public decimal TotalPecas { get; set; }
        public int QtdOS { get; set; }
        public string Label => $"{Mes:D2}/{Ano}";
    }

    public class ResumoGeral
    {
        public int QtdOS { get; set; }
        public decimal TotalMaoDeObra { get; set; }
        public decimal TotalPecas { get; set; }
        public decimal TotalGeral { get; set; }
    }
}
