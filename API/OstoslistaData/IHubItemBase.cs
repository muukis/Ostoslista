using System;

namespace OstoslistaData
{
    public interface IHubItemBase
    {
        Guid? Id { get; set; }
        ShopperEntity Shopper { get; set; }
    }
}