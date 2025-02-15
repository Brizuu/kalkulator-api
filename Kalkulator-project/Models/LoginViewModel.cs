using System.ComponentModel.DataAnnotations;

namespace Kalkulator_project.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(20, ErrorMessage = "Username cannot be longer than 20 characters")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MaxLength(20, ErrorMessage = "Password cannot be longer than 20 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}