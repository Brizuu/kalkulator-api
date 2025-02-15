using System.ComponentModel.DataAnnotations;

namespace Kalkulator_project.Entities;

public class UserServices
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "User ID is required")]
    [MaxLength(20, ErrorMessage = "User ID cannot be longer than 20 characters")]
    public string UserId { get; set; }
    
    [Required(ErrorMessage = "Service ID is required")]
    [MaxLength(20, ErrorMessage = "Service ID cannot be longer than 20 characters")]
    public int ServiceId { get; set; }
    
    [Required(ErrorMessage = "Total cost is required")]
    public decimal TotalCost { get; set; }
    
    [Required(ErrorMessage = "Area is required")]
    public decimal Area { get; set; }
    
    [Required(ErrorMessage = "Date is required")]
    public DateTime CalculationDate { get; set; } = DateTime.UtcNow;
}