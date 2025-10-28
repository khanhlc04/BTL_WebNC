using System.Collections.Concurrent;
using BTL_WebNC.Models.Chat;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BTL_WebNC.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();
        private readonly IChatRepository _chatRepository;
        private readonly IAccountRepository _accountRepository;

        public ChatHub(IChatRepository chatRepository, IAccountRepository accountRepository)
        {
            _chatRepository = chatRepository;
            _accountRepository = accountRepository;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendPrivateMessage(
            string fromUserEmail,
            string toUserEmail,
            string message
        )
        {
            var toConnectionId = _connections.FirstOrDefault(x => x.Value == toUserEmail).Key;

            // persist chat to DB if accounts exist
            var fromAccount = await _accountRepository.GetByEmailAsync(fromUserEmail);
            var toAccount = await _accountRepository.GetByEmailAsync(toUserEmail);
            if (fromAccount != null && toAccount != null)
            {
                var chat = new ChatModel
                {
                    Message = message,
                    SenderId = fromAccount.Id,
                    ReceiverId = toAccount.Id,
                    CreatedAt = DateTime.Now,
                };
                await _chatRepository.CreateAsync(chat);
            }

            if (toConnectionId != null)
            {
                // deliver to recipient only; caller will render locally to avoid duplicate
                await Clients
                    .Client(toConnectionId)
                    .SendAsync("ReceivePrivateMessage", fromUserEmail, message);
            }
            else
            {
                await Clients.Caller.SendAsync("UserNotFound", toUserEmail);
            }
        }

        public async Task JoinChat(string username)
        {
            _connections[Context.ConnectionId] = username;
            await Clients.All.SendAsync("UserJoined", username);
            await Clients.Caller.SendAsync("UpdateUserList", _connections.Values.ToList());
            await Clients.Others.SendAsync("UpdateUserList", _connections.Values.ToList());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
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
