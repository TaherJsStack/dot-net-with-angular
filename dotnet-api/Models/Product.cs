using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dotnetAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ProductNameRequired")]
        [StringLength((5), ErrorMessage = "ProductNameMaxLength")]

        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "PriceRequired")]
        [Range(0.01, double.MaxValue, ErrorMessage = "PriceRange")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "ProductStockRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "StockMinLength")]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "LengthRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "LengthMinLength")]
        public int Length { get; set; }

        [Required(ErrorMessage = "WidthRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "WidthMinLength")]
        public int Width { get; set; }

        [Required(ErrorMessage = "HeightRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "HeightMinLength")]
        public int Height { get; set; }

        [Required(ErrorMessage = "WeightRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "WeightMinLength")]
        public int Weight { get; set; }

        [Required(ErrorMessage = "ColorRequired")]
        public string Color { get; set; } 

        [Required(ErrorMessage = "MaterialRequired")]
        public string Material { get; set; }

        [Required(ErrorMessage = "ManufacturerRequired")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "WarrantyRequired")]
        public string Warranty { get; set; } 

        public bool ActiveState { get; set; }

        [Required(ErrorMessage = "CreatedAtRequired")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "UpdatedAtRequired")]
        public DateTime UpdatedAt { get; set; }

        [Required(ErrorMessage = "CategoryIdRequired")]
        public int CategoryId { get; set; }

        [JsonIgnore] // Ignore during serialization to prevent circular references
        public Category? Category { get; set; }
    }
}