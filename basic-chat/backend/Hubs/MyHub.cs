using backend.Managers;
using backend.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace backend.Hubs
{
    public class MyHub : Hub
    {
        private UserListManager _userListManager;
        public MyHub(UserListManager userListManager) 
        {
            _userListManager = userListManager;
        }
        public bool ConnectChat(string name)
        {
            if(_userListManager.Users.FirstOrDefault(u => u.ConnectionID == Context.ConnectionId) != null)
            {
                return false;
            }

            _userListManager.AddUser(new User { Name = name, ConnectionID = Context.ConnectionId });
            return true;
        }

        public async void SendAll(string message)
        {
            var user = _userListManager.GetUser(Context.ConnectionId);
            await Clients.All.SendAsync("GetMessage", message, user!.Name);

            // If you want to handle caller message sending in frontend app and optimize
            // Websocket performance, you can use same tricks like this.

            // await Clients.Others.SendAsync("GetMessage", message);
        }

        public async void SendToGroup(string groupName, string message)
        {
            var user = _userListManager.GetUser(Context.ConnectionId);
            await Clients.Group(groupName).SendAsync("GetMessage", $"{groupName} - {user!.Name}: {message}");

            // If you want to handle caller message sending in frontend app and optimize
            // Websocket performance, you can use same tricks like this.

            // await Clients.OthersInGroup(groupName).SendAsync("GetMessage", message);
        }

        public async void JoinToGroup(string groupName)
        {
            var user = _userListManager.GetUser(Context.ConnectionId);
            if(user != null)
            {
                var group = _userListManager.Groups.FirstOrDefault(x => x.Name == groupName);

                if(group == null)
                {
                    group = new Group()
                    {
                        Name = groupName,
                        Users = new List<User> { user }
                    };
                    _userListManager.Groups.Add(group);
                    await Clients.All.SendAsync("NewGroupAdded", groupName);
                }
                else
                {
                    group.Users.Add(user);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName).SendAsync("UserJoinedToGroup", group.Name, user.Name);

                var myGroups = _userListManager.GetUserGroupsWithConnectionId(Context.ConnectionId);
                await Clients.Caller.SendAsync("GetMyGroups", _userListManager.GetUserGroupsWithConnectionId(Context.ConnectionId));
            }
        }

        public List<Group> GetMyGroups()
        {
            return _userListManager.GetUserGroupsWithConnectionId(Context.ConnectionId);
        }

        public async void RemoveFromGroup(string groupName)
        {
            var user = _userListManager.GetUser(Context.ConnectionId);
            var group = _userListManager.Groups.First(x => x.Name == groupName);
            if (user != null && group != null)
            {
                group.Users.Remove(user);
                if (group.Users.Count > 0)
                {
                    _userListManager.Groups.Remove(group);
                    await Clients.Group(groupName).SendAsync("GroupDeleted", group.Name);
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName).SendAsync("UserDisconnectedFromGroup", group.Name, user.Name);

                var myGroups = _userListManager.GetUserGroupsWithConnectionId(Context.ConnectionId);
                await Clients.Caller.SendAsync("GetMyGroups", _userListManager.GetUserGroupsWithConnectionId(Context.ConnectionId));
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _userListManager.RemoveUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
