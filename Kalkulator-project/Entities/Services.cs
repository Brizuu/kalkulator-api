using System.ComponentModel.DataAnnotations;

namespace Kalkulator_project.Entities;

public class Services
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Service name is required")]
    [MaxLength(100, ErrorMessage = "Service name cannot be longer than 100 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Service price is required")]
    [MaxLength(50, ErrorMessage = "Service price cannot be longer than 50 characters")]
    public decimal Price { get; set; }
}