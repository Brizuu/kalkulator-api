using System.ComponentModel.DataAnnotations;

namespace Kalkulator_project.Models;

public class UserServicesViewModel
{
    [Required(ErrorMessage = "User ID is required")]
    [MaxLength(20, ErrorMessage = "User ID cannot be longer than 20 characters")]
    public int UserId { get; set; }
    
    [Required(ErrorMessage = "Service ID is required")]
    [MaxLength(20, ErrorMessage = "Service ID cannot be longer than 20 characters")]
    public int ServiceId { get; set; }
}