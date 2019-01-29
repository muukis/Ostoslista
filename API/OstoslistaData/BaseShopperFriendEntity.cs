using System;
using System.Collections.Generic;
using System.Text;

namespace OstoslistaData
{
    public abstract class BaseShopperFriendEntity : BaseShopperChildEntity, IEmail
    {
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Name { get; set; }
    }
}
