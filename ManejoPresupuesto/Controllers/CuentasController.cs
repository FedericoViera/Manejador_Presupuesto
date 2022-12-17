using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using ManejoPresupuesto.Servicios.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IServicioUsuarios _servicio_Usuarios;
        private readonly IMapper _mapper;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
                                 IRepositorioCuentas repositorioCuentas,
                                 IServicioUsuarios servicioUsuarios,
                                 IMapper mapper) {

            _repositorioTiposCuentas = repositorioTiposCuentas;
            _repositorioCuentas = repositorioCuentas;
            _servicio_Usuarios = servicioUsuarios;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicio_Usuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await _repositorioCuentas.Listar(usuarioId);

            var modelo = cuentasConTipoCuenta.GroupBy(x => x.TipoCuenta)
                                             .Select(grupo => new IndiceCuentasViewModel
                                             {
                                                 TipoCuenta = grupo.Key,
                                                 Cuentas = grupo.AsEnumerable()
                                             }).ToList();
            return View(modelo);
        }
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _servicio_Usuarios.ObtenerUsuarioId();
            var modelo = new CuentaViewModel
            {
                TiposCuentas = await ObtenerTiposCuentas(usuarioId)
            };
            
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaViewModel cuenta)
        {
            var usuarioId = _servicio_Usuarios.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await _repositorioCuentas.Crear(cuenta);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicio_Usuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = _mapper.Map<CuentaViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaViewModel cuentaEditar)
        {
            var usuarioId = _servicio_Usuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null) { return RedirectToAction("NoEncontrado", "Home"); }

            var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if (tipoCuenta is null) { return RedirectToAction("NoEncontrado", "Home"); }

            await _repositorioCuentas.Editar(new Cuenta
            {
                Id = cuenta.Id,
                TipoCuentaId = cuentaEditar.TipoCuentaId,
                Nombre = cuentaEditar.Nombre,
                Balance = cuentaEditar.Balance,
                Descripcion = cuenta.Descripcion
            });
            return RedirectToAction("Index");
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await _repositorioTiposCuentas.Listar(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
