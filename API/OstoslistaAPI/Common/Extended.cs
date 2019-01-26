using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using OstoslistaData;

namespace OstoslistaAPI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extended
    {
        /// <summary>
        /// Get authenticated user image URL
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserImageUrl(this ClaimsPrincipal user, int size = 20)
        {
            var retval = user.Claims.Single(o => o.Type == "profileImg").Value;
            return Regex.Replace(retval, @"\?sz=\d+", $"?sz={size}");
        }

        /// <summary>
        /// Get authenticated user email address
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }

        /// <summary>
        /// Get authenticated user email identifier
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserEmailIdentifier(this ClaimsPrincipal user)
        {
            var email = user.GetUserEmail();

            if (email == null)
            {
                return null;
            }

            return $"{user.Identity.AuthenticationType}#{email}";
        }

        /// <summary>
        /// Get users read authorization for a shopper
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopper"></param>
        /// <returns></returns>
        public static bool GetShopperReadAuthorization(this ClaimsPrincipal user, ShopperEntity shopper)
        {
            var userEmailAddressId = user.GetUserEmailIdentifier();

            return shopper?.Email == null ||
                   (shopper.PublicReadAccess ?? false) ||
                   (shopper.PublicWriteAccess ?? false) ||
                   string.Equals(shopper.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase) ||
                   (shopper.FriendReadAccess ?? false) ||
                   (shopper.FriendWriteAccess ?? false) ||
                   shopper.Friends.Any(o => string.Equals(o.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get users write authorization for a shopper
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopper"></param>
        /// <returns></returns>
        public static bool GetShopperWriteAuthorization(this ClaimsPrincipal user, ShopperEntity shopper)
        {
            var userEmailAddressId = user.GetUserEmailIdentifier();

            return shopper?.Email == null ||
                   (shopper.PublicWriteAccess ?? false) ||
                   shopper.Email == userEmailAddressId ||
                   (shopper.FriendWriteAccess ?? false) ||
                   shopper.Friends.Any(o => string.Equals(o.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
