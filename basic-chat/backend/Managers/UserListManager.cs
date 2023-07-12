using backend.Models;

namespace backend.Managers
{
    public class UserListManager
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Group> Groups { get; set; } = new List<Group>();

        public void AddUser(User user)
        {
            Users.Add(user);
        }
        public User? GetUser(string connectionId)
        {
            return Users.FirstOrDefault(x => x.ConnectionID == connectionId);
        }

        public void RemoveUser(string connectionId)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionID == connectionId);
            if(user != null)
            {
                Users.Remove(user);
            }
        }

        public List<Group> GetUserGroupsWithConnectionId(string connectionId)
        {
            return Groups.Where(x => x.Users.Any(y => y.ConnectionID == connectionId)).ToList();
        }
    }
}
