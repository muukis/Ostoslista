using System;

namespace OstoslistaData
{
    public class ShoppingListItemEntity : BaseEntity
    {
        public string Title { get; set; }
        public bool? Pending { get; set; }
        public virtual ShopperEntity Shopper { get; set; }
        public Guid ShopperId { get; set; }
    }
}
