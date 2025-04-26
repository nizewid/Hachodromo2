using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "El campo {0} no es un correo electrónico válido.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener mínimo {1} caracteres.")]
        public string Password { get; set; } = null!;
    }
}
