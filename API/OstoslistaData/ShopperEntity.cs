using System;
using System.Collections.Generic;
using OstoslistaInterfaces;

namespace OstoslistaData
{
    public class ShopperEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<ShoppingListItemEntity> Items { get; set; }
    }
}
