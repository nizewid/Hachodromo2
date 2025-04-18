using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name="País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100,ErrorMessage = "El campo {0} No puede tener más de {1} carácteres")]
        public string Name { get; set; } = null!;


    }
}
