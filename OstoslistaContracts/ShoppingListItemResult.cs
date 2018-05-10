using System;

namespace OstoslistaContracts
{
    public class ShoppingListItemResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool InCart { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
