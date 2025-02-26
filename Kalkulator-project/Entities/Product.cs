using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kalkulator_project.Entities;

[Table("products")]
public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public bool OnSale { get; set; }
    
    [ForeignKey("Seller")]
    public int SellerId { get; set; }
    
    [ForeignKey("Specification")]
    public int SpecificationId { get; set; }
    public string Image { get; set; }

    public virtual Specification Specification { get; set; }
}