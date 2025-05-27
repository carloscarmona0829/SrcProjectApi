using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SrcProject.Models.Security
{
    public class ApplicationUserModel : IdentityUser
    {
        // Propiedades personalizadas que modifican la tabla AspNetUsers de la Base de datos de Identity

        [Required(ErrorMessage = "El Número de Identificación es obligatorio.")] 
        public string Dni { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string LastName { get; set; }
        public string BirthDay { get; set; }
    }
}
