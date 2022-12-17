using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios.Repositorios
{
    public interface IRepositorioTiposCuentas
    {
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task Editar(TipoCuenta tipoCuenta);
        Task<bool> Existe(TipoCuenta tipoCuentaViewModel);
        Task<IEnumerable<TipoCuenta>> Listar(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string _connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre,UsuarioId,Orden)
                                                             VALUES (@Nombre, @UsuarioID, @Orden);
                                                             SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                    FROM TiposCuentas
                                                                    WHERE Nombre = @Nombre AND UsuarioId = @UsuarioID;",
                                                                    new { tipoCuenta.Nombre, tipoCuenta.UsuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Listar(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            var lista = await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas
                                                                           WHERE UsuarioId = @UsuarioId ORDER BY Orden",
                                                                           new { usuarioId });

            return lista;
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas                                                                        
                                                                        WHERE Id = @Id AND UsuarioId = @UsuarioId;",
                                                                        new {id , usuarioId});
        }

        public async Task Editar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id;", tipoCuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"DELETE FROM TiposCuentas
                                            WHERE Id = @Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
