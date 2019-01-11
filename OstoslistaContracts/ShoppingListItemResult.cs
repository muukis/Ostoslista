using System;

namespace OstoslistaContracts
{
    public class ShoppingListItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShopperName { get; set; }
        public bool Pending { get; set; }
    }
}
