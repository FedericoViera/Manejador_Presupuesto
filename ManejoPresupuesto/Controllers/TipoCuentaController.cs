using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using ManejoPresupuesto.Servicios.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class TipoCuentaController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServicioUsuarios _servicioUsuarios;

        public TipoCuentaController(IRepositorioTiposCuentas repositorioTiposCuentas,
                                    IServicioUsuarios servicioUsuarios)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
        }   

        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var listaTiposCuentas = await _repositorioTiposCuentas.Listar(usuarioId);
            return View(listaTiposCuentas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await _repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Editar(tipoCuenta);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {            
            if(!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            tipoCuenta.UsuarioId = usuarioId;

            var existeTipoCuenta = await _repositorioTiposCuentas.Existe(tipoCuenta);

            if (existeTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                       $"El nombre {tipoCuenta.Nombre} ya existe.");

                return View(tipoCuenta);
            }
                
            var listaTiposCuentas = await _repositorioTiposCuentas.Listar(usuarioId);
            var indiceUltimo = 0;
            if (listaTiposCuentas.Any())
            {
                indiceUltimo = listaTiposCuentas.Last().Orden;
            }
            tipoCuenta.Orden = indiceUltimo + 1;

            await _repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Existe(string nombre)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var existeTipoCuenta = await _repositorioTiposCuentas.Existe(
                                   new TipoCuenta { Nombre = nombre, UsuarioId = usuarioId });
            if (existeTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe.");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var comprobarTipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (comprobarTipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await _repositorioTiposCuentas.Listar(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentaNoPertenecenUsuario = ids.Except(idsTiposCuentas);

            if (idsTiposCuentaNoPertenecenUsuario.Any()) {

                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select( (valor, indice) =>            
            new TipoCuenta() { Id = valor, Orden = indice + 1 } )
                .AsEnumerable();

            await _repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }

    }
}
