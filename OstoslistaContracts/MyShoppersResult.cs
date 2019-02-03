using System.Collections.Generic;

namespace OstoslistaContracts
{
    public class MyShoppersResult
    {
        public IEnumerable<MyShopperResult> MyShoppers { get; set; }
        public IEnumerable<FriendShoppersResult> FriendShoppers { get; set; }
        public IEnumerable<FriendRequestedShoppersResult> FriendRequestedShoppers { get; set; }
    }
}
