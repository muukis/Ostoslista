using System.ComponentModel.DataAnnotations;

namespace OstoslistaContracts
{
    public class ShopperNameDto
    {
        [Required]
        public string Name { get; set; }
    }
}
