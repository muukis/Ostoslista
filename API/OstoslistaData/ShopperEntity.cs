﻿using System.Collections.Generic;

namespace OstoslistaData
{
    public class ShopperEntity : BaseShopperSettingsEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<ShoppingListItemEntity> Items { get; set; }
        public virtual ICollection<ShopperFriendEntity> Friends { get; set; }
        public virtual ICollection<ShopperFriendRequestEntity> FriendRequests { get; set; }
    }
}
