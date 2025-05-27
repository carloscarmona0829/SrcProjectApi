using System.ComponentModel.DataAnnotations;

namespace SrcProject.Models.InModels.Security
{
    public class ResetPasswordIM
    {
        [Required(ErrorMessage = "El campo Token es obligatorio.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Email debe ser un email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo NewPassword es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El campo NewPassword debe tener entre 5 y 50 caracteres.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "El campo ConfirmPassword es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El campo ConfirmPassword debe tener entre 5 y 50 caracteres.")]
        public string ConfirmPassword { get; set; }
    }
}
