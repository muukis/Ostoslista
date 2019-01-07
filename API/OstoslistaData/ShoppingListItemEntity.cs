using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OstoslistaInterfaces;

namespace OstoslistaData
{
    [Table("Ostoslista")]
    public class ShoppingListItemEntity : IShoppingListItem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public bool? Pending { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
