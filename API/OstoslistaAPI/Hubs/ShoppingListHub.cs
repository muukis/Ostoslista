using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OstoslistaContracts;

namespace OstoslistaAPI.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class ShoppingListHub : Hub<IMessages>
    {
        private readonly Dictionary<string, string> _groupConnections = new Dictionary<string, string>();

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
