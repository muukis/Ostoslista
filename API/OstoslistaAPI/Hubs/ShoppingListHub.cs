using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OstoslistaData;

namespace OstoslistaAPI.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class ShoppingListHub : Hub<IMessages>
    {
        private readonly IShoppingListService _service;
        private readonly Dictionary<string, string> _groupConnections = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public ShoppingListHub(IShoppingListService service)
        {
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveContextFromGroup();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task RemoveContextFromGroup()
        {
            if (_groupConnections.ContainsKey(Context.ConnectionId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, _groupConnections[Context.ConnectionId]);
                _groupConnections.Remove(Context.ConnectionId);
            }
        }

        private async Task AddContextToGroup(string shopperName)
        {
            await RemoveContextFromGroup();
            await Groups.AddToGroupAsync(Context.ConnectionId, shopperName);
            _groupConnections.Add(Context.ConnectionId, shopperName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperName"></param>
        /// <returns></returns>
        public async Task RegisterShopper(string shopperName)
        {
            await AddContextToGroup(shopperName);
        }
    }
}
