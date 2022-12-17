using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta   // : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de la cuenta")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [PrimeraLetraMayuscula]
        [Remote(action:"Existe", controller: "TipoCuenta")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }


        //Validaciones a nivel de Propiedad, quitando el new[] { nameof } hariamos la validacion
        //a nivel de Modelo, por ejemplo si fuera un tema de 

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            if (Nombre != null && Nombre.Length > 0)
//            {
//                var primeraLetra = Nombre[0].ToString();

//                if (primeraLetra != primeraLetra.ToUpper())
//                {
//                    yield return new ValidationResult("La primera letra debe ser mayúscula.",
//                        new[] { nameof(Nombre) });
//                }
//            }
//        }
   }
}
