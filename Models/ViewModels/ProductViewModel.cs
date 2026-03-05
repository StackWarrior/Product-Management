using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Product_Management.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

            [Required(ErrorMessage = "Product name is required")]
            [StringLength(100, MinimumLength = 2)]
            [Display(Name = "Product Name")]
            [Remote(action: "IsProductNameAvailable", controller: "Products",
                    AdditionalFields = "Id",
                    ErrorMessage = "This product name already exists!")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Description is required")]
            [StringLength(500)]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
            [Display(Name = "Price ($)")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Stock is required")]
            [Range(0, 10000)]
            [Display(Name = "Stock Quantity")]
            public int Stock { get; set; }

            [Required(ErrorMessage = "Please select a category")]
            [Display(Name = "Category")]
            public int CategoryId { get; set; }

            [Display(Name = "Product Image")]
            public IFormFile? ImageFile { get; set; }

            public string? ExistingImagePath { get; set; }
        }
    }

