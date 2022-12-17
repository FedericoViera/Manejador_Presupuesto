using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios.Repositorios
{
    public interface IRepositorioCuentas
    {
        Task Crear(Cuenta cuenta);
        Task Editar(Cuenta cuenta);
        Task<IEnumerable<Cuenta>> Listar(int usuarioId);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string _connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
                                                            VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                                                            SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        public async Task Editar(Cuenta cuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.ExecuteAsync(@"UPDATE Cuentas
                                                    SET Nombre = @Nombre, Balance = Balance,
                                                    Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                                                    WHERE Id = @Id;", cuenta);
        }

        public async Task<IEnumerable<Cuenta>> Listar(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT c.Id, c.Nombre, c.Balance, tc.Nombre as TipoCuenta
                                                        FROM Cuentas as c
                                                        INNER JOIN TiposCuentas tc 
                                                        on c.TipoCuentaId = tc.Id
                                                        WHERE tc.UsuarioId = @UsuarioId
                                                        ORDER BY tc.Orden;", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT c.Id,  c.Nombre, c.Balance, c.Descripcion, c.TipoCuentaId
                                                                       FROM Cuentas as c
                                                                       INNER JOIN TiposCuentas as tc
                                                                       ON c.TipoCuentaId = tc.Id
                                                                       WHERE tc.UsuarioId = @UsuarioId AND c.Id = @Id;",
                                                                       new {id, usuarioId});
        }
    }
}
