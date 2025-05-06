using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.DTOs
{
    public class ReservationDto
    {
        public int? Id { get; set; } // null en creación, valor en edición

        [Required]
        public int SiteId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Debes indicar una cantidad de personas válida.")]
        public int PersonCount { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; }

        [Required]
        public TimeSpan HourStart { get; set; }

        [Required]
        public TimeSpan HourEnd { get; set; }

        public string? Remarks { get; set; }
    }

}
