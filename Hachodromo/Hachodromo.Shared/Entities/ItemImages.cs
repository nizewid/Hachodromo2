using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Entities
{
    public class ItemImage
    {
        public int Id { get; set; }

        public Item Item{ get; set; } = null!;

        public int ItemId { get; set; }

        [Display(Name = "Imagen")]
        public string Image { get; set; } = null!;

    }
}
