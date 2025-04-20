using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hachodromo.Shared.Entities
{
    public class Region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Desactiva el autoincremento
        public int RegionId { get; set; } // Clave primaria sin autoincremento

        [Display(Name = "Región")]
        [Required(ErrorMessage = "El nombre de la región es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la región no puede superar los 100 caracteres.")]
        public string RegionName { get; set; } = null!;// Nombre de la región

        [Required(ErrorMessage = "El País es obligatorio.")]
        public int CountryId { get; set; } // Clave foránea a la tabla de países

        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; } = null!;

        [InverseProperty("Region")]
        public virtual ICollection<City> Cities { get; set; } = new List<City>();

    }
}
