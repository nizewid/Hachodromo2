using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Membresia")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [MinLength(3, ErrorMessage = "El campo {0} debe tener mínimo {1} caractéres.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Price { get; set; }

        [Display(Name = "Duración")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El campo {0} debe estar entre {1} y {2}.")]
        [DisplayFormat(DataFormatString = "{0} meses")]
        public int Duration { get; set; }

        [Display(Name = "Descuento")]
        [Range(0.0, 1.0, ErrorMessage = "El campo {0} debe estar entre 0.0 y 1.0.")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        [Column(TypeName = "decimal(5,4)")]
        public decimal Discount { get; set; }


        public ICollection<User>? Users { get; set; } = [];
    }
}
