using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OstoslistaAPI.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessages
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="title"></param>
        /// <param name="isPending"></param>
        /// <returns></returns>
        Task NewItemCreated(Guid itemId, string title, bool isPending);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="isPending"></param>
        /// <returns></returns>
        Task ItemPendingChanged(Guid itemId, bool isPending);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        Task RemoveItem(Guid itemId);
    }
}
