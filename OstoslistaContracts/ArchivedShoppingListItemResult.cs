using System;

namespace OstoslistaContracts
{
    public class ArchivedShoppingListItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShopperName { get; set; }
        public DateTime Archived { get; set; }
    }
}
