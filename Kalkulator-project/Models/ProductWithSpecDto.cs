using System.ComponentModel.DataAnnotations;

namespace Kalkulator_project.Models;

public class ProductWithSpecDto
{
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool OnSale { get; set; }
    public int SellerId { get; set; }
    public int SpecificationId { get; set; }  
    public SpecificationDto Specification { get; set; }
}

public class SpecificationDto
{
    public string BrandName { get; set; }
    public string Model { get; set; }
    public string FuelType { get; set; }
    public string ProductionDate { get; set; }
    public int Mileage { get; set; }
}
