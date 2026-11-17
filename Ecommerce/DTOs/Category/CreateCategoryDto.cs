using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}