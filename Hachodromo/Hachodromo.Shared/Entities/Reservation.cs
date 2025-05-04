using Hachodromo.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [Range(1, 4, ErrorMessage = "El campo {0} debe estar entre {1} y {2}.")]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending; // 1 = Pendiente , 2= Confirmada , 3= Cancelada, 4 = En curso

        [Required]
        [Display(Name = "fecha de la reserva")]
        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "Fecha de creación")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Hora de inicio")]
        public TimeSpan HourStart { get; set; }

        [Required]
        [Display(Name = "Hora de fin")]
        public TimeSpan HourEnd { get; set; }

        [Display(Name = "Detalles adicionales")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Remarks { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<ReservationTarget> ReservationTargets { get; set; } = new List<ReservationTarget>();

    }
}
