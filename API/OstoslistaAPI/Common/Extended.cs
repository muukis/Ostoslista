using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
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
            return !user.Identity.IsAuthenticated ? null : user.FindFirstValue(ClaimTypes.Email);
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
                   (((shopper.FriendReadAccess ?? false) ||
                   (shopper.FriendWriteAccess ?? false)) &&
                   shopper.Friends.Any(o => string.Equals(o.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase)));
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
                   string.Equals(shopper.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase) ||
                   ((shopper.FriendWriteAccess ?? false) &&
                   shopper.Friends.Any(o => string.Equals(o.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Get users owner authorization for a shopper
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopper"></param>
        /// <returns></returns>
        public static bool GetShopperOwnerAuthorization(this ClaimsPrincipal user, ShopperEntity shopper)
        {
            var userEmailAddressId = user.GetUserEmailIdentifier();

            return shopper?.Email != null &&
                   string.Equals(shopper.Email, userEmailAddressId, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get API authorization bypass password from request headers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetApiAuthorizationBypassPassword(this HttpRequest request)
        {
            return request.Headers["apiPassword"];
        }

        /// <summary>
        /// Check API password from request headers and check if it matches shopper API password
        /// </summary>
        /// <param name="shopper"></param>
        /// <returns>True if shopper password is set (not null or empty) and the passwords match</returns>
        public static bool BypassAuthentication(this ShopperEntity shopper)
        {
            return string.IsNullOrEmpty(shopper.ApiAuthorizationBypassPassword) ||
                   string.Equals(shopper.ApiAuthorizationBypassPassword,
                       ApiHttpContext.Current.Request.GetApiAuthorizationBypassPassword());
        }
    }
}
