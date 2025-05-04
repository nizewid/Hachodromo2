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
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public int SiteId { get; set; }

        [Display(Name = "Capacidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Capacity { get; set; } = 6;

        public TargetStatus Status { get; set; } = TargetStatus.Available; // 1 = Disponible , 2= Con reservas , 3= en mantenimiento, 4 = En uso

        [ForeignKey("SiteId")]
        public virtual Site? Site { get; set; } = null!;
        public ICollection<ReservationTarget> ReservationTargets { get; set; } = new List<ReservationTarget>();

    }
}
