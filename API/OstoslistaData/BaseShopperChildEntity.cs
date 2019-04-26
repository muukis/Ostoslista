using System;

namespace OstoslistaData
{
    public abstract class BaseShopperChildEntity : BaseEntity, IHubItemBase
    {
        public virtual ShopperEntity Shopper { get; set; }
        public Guid ShopperId { get; set; }
    }
}
