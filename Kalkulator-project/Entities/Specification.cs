using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kalkulator_project.Entities;

[Table("specs")]
public class Specification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string BrandName { get; set; }

    [Required]
    public string Model { get; set; }

    public string FuelType { get; set; }

    public string ProductionDate { get; set; }

    public int Mileage { get; set; }
}