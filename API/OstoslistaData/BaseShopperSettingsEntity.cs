namespace OstoslistaData
{
    public abstract class BaseShopperSettingsEntity : BaseEntity
    {
        public bool? AllowNewFriendRequests { get; set; }
        public bool? PublicWriteAccess { get; set; }
        public bool? PublicReadAccess { get; set; }
        public bool? FriendWriteAccess { get; set; }
        public bool? FriendReadAccess { get; set; }
    }
}
