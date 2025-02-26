namespace Kalkulator_project.Models
{
    public class ProductWithSpecAndImageDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool OnSale { get; set; }
        public int SellerId { get; set; }
        public int SpecificationId { get; set; } 
        public SpecificationDto Specification { get; set; }
        public string Image { get; set; } 
    }
}