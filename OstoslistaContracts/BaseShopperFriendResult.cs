using System;

namespace OstoslistaContracts
{
    public abstract class BaseShopperFriendResult
    {
        public Guid Id { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Name { get; set; }
    }
}
