using System;

namespace OstoslistaData
{
    public abstract class BaseShopperChildEntity : BaseEntity, IHubItemRemove
    {
        public virtual ShopperEntity Shopper { get; set; }
        public Guid ShopperId { get; set; }
    }
}
