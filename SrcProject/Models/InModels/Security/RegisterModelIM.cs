using System.ComponentModel.DataAnnotations;

namespace SrcProject.Models.InModels.Security
{
    public class RegisterModelIM
    {
        [Required(ErrorMessage = "El campo UserName es obligatorio.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "El campo UserName debe tener entre 6 y 50 caracteres.")]
        public string UserName { get; set; }

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
