using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
        public static string GetUserImageUrl(this ClaimsPrincipal user)
        {
            var retval = user.Claims.Single(o => o.Type == "profileImg").Value;
            return Regex.Replace(retval, @"\?sz=\d+", "?sz=20");
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
    }
}
