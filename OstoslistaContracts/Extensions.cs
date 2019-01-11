using System;
using System.Collections.Generic;
using System.Linq;
using OstoslistaData;

namespace OstoslistaContracts
{
    public static class Extensions
    {
        public static IEnumerable<ShoppingListItemResult> ToResults(this IEnumerable<ShoppingListItemEntity> items)
        {
            return items.Select(o => o.ToResult());
        }
        public static ShoppingListItemResult ToResult(this ShoppingListItemEntity item)
        {
            return new ShoppingListItemResult
            {
                Id = item.Id ?? Guid.Empty,
                Title = item.Title,
                Pending = item.Pending ?? true,
                ShopperName = item.Shopper.Name
            };
        }
    }
}
