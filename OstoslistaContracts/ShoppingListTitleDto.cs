using System.ComponentModel.DataAnnotations;

namespace OstoslistaContracts
{
    public class ShoppingListTitleDto
    {
        [Required]
        public string Title { get; set; }
    }
}
