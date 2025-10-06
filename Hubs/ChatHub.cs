using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BTLChatDemo.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendPrivateMessage(string fromUser, string toUser, string message)
        {
            var toConnectionId = _connections.FirstOrDefault(x => x.Value == toUser).Key;
            if (toConnectionId != null)
            {
                await Clients.Client(toConnectionId).SendAsync("ReceivePrivateMessage", fromUser, message);
                await Clients.Caller.SendAsync("ReceivePrivateMessage", $"You to {toUser}", message);
            }
            else
            {
                await Clients.Caller.SendAsync("UserNotFound", toUser);
            }
        }

        public async Task JoinChat(string username)
        {
            _connections[Context.ConnectionId] = username;
            await Clients.All.SendAsync("UserJoined", username);
            await Clients.Caller.SendAsync("UpdateUserList", _connections.Values.ToList());
            await Clients.Others.SendAsync("UpdateUserList", _connections.Values.ToList());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryRemove(Context.ConnectionId, out string username))
            {
                await Clients.All.SendAsync("UserLeft", username);
                await Clients.All.SendAsync("UpdateUserList", _connections.Values.ToList());
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}