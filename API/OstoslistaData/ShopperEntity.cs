using System.Collections.Generic;

namespace OstoslistaData
{
    public class ShopperEntity : BaseShopperSettingsEntity, IEmail
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual ICollection<ShoppingListItemEntity> Items { get; set; }
        public virtual ICollection<ShopperFriendEntity> Friends { get; set; }
        public virtual ICollection<ShopperFriendRequestEntity> FriendRequests { get; set; }
        public virtual ICollection<ArchivedShoppingListItemEntity> ArchivedItems { get; set; }
    }
}
