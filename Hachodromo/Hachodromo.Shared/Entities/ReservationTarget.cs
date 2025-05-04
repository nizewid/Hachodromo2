using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class ReservationTarget
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int ReservationId { get; set; }

        [Required]
        public int TargetId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; } = null!;

        [ForeignKey("TargetId")]
        public virtual Target Target { get; set; } = null!;
    }
}
