namespace OstoslistaContracts
{
    public class ShopperResult
    {
        public string ShopperName { get; set; }
        public int ItemCount { get; set; }
        public int FriendRequestCount { get; set; }
        public int FriendCount { get; set; }
        public bool AllowNewFriends { get; set; }
        public bool PublicReadAccess { get; set; }
        public bool PublicWriteAccess { get; set; }
        public bool FriendReadAccess { get; set; }
        public bool FriendWriteAccess { get; set; }
    }
}
