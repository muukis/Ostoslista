using System;

namespace OstoslistaData
{
    public class ArchivedShoppingListItemEntity : IHubItemRemove
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public DateTime? Archived { get; set; }
        public virtual ShopperEntity Shopper { get; set; }
        public Guid ShopperId { get; set; }
    }
}
