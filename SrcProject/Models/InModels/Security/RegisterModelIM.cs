using System.ComponentModel.DataAnnotations;

namespace SrcProject.Models.InModels.Security
{
    public class RegisterModelIM
    {
        [Required(ErrorMessage = "El campo Dni es obligatorio.")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "El campo Dni debe tener entre 6 y 15 caracteres.")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El campo FirstName es obligatorio.")]        
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo LastName es obligatorio.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El campo UserName es obligatorio.")]        
        public string UserName { get; set; }

        public string? BirthDay { get; set; }
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Email debe ser un email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Password es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El campo Password debe tener entre 5 y 50 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo ConfirmPassword es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El campo ConfirmPassword debe tener entre 5 y 50 caracteres.")]
        public string ConfirmPassword { get; set; }
    }
}
