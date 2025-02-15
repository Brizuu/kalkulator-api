using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Kalkulator_project.Entities;

[Index(nameof(Username), IsUnique = true)]

public class UserAccount
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(20, ErrorMessage = "Username cannot be longer than 20 characters")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MaxLength(20, ErrorMessage = "Password cannot be longer than 20 characters")]
    public string Password { get; set; }
    
}