using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hachodromo.Shared.Entities
{
    public partial class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CityCode { get; set; }
        [Required]
        public int RegionId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "No puede superar los 100 Caracteres")]
        public string CityName { get; set; } = "CityName";

        [ForeignKey("RegionId")]
        [InverseProperty("Cities")]
        public virtual Region Region { get; set; } = null!;

    }
}