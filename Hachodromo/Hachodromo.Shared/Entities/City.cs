using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hachodromo.Shared.Entities
{
    public partial class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityId { get; set; }
        [Required]
        public int RegionId { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage ="El Campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "el campo {0} puede superar los {1} Caracteres")]
        public string CityName { get; set; } = "CityName";

        [ForeignKey("RegionId")]
        [InverseProperty("Cities")]
        public virtual Region? Region { get; set; }

        public ICollection<User>? Users { get; set; }

    }
}