using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.DTOs
{
    public class ImageDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public List<string> Images { get; set; } = null!;
    }

}
