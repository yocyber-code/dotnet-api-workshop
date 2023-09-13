using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cmdev_dotnet_api.DTOs.Product
{
    public class ProductRequest
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Max length is 100")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000")]
        public int Stock { get; set; }

        [Required]
        [Range(0, 1_000_000, ErrorMessage = "Stock must be between 0 and 1000")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<IFormFile>? FormFiles { get; set; }
    }
}