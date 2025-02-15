using System.ComponentModel.DataAnnotations;


namespace Kalkulator_project.Models;

public class ServicesViewModel
{
    [Required(ErrorMessage = "Service ID is required")]
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Area is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Area must be greater than 0")]
    public decimal Area { get; set; }
    
    [Required(ErrorMessage = "TotalCost is required")]
    public decimal TotalCost { get; set; }
}