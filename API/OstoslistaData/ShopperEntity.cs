using System;
using System.Collections.Generic;
using OstoslistaInterfaces;

namespace OstoslistaData
{
    public class ShopperEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? AllowNewFriendRequests { get; set; }
        public bool? PublicWriteAccess { get; set; }
        public bool? PublicReadAccess { get; set; }
        public bool? FriendWriteAccess { get; set; }
        public bool? FriendReadAccess { get; set; }
        public virtual ICollection<ShoppingListItemEntity> Items { get; set; }
        public virtual ICollection<ShopperFriendEntity> Friends { get; set; }
        public virtual ICollection<ShopperFriendRequestEntity> FriendRequests { get; set; }
    }
}
