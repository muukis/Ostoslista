namespace OstoslistaContracts
{
    public class MyShopperResult : BaseExtendedShoppersResult
    {
        public int FriendRequestCount { get; set; }
        public int FriendCount { get; set; }
        public bool AllowNewFriends { get; set; }
        public bool PublicReadAccess { get; set; }
        public bool PublicWriteAccess { get; set; }
        public bool FriendReadAccess { get; set; }
        public bool FriendWriteAccess { get; set; }
    }
}
