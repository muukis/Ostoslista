using System;

namespace OstoslistaData
{
    public interface IHubItemRemove
    {
        Guid? Id { get; set; }
        ShopperEntity Shopper { get; set; }
    }
}