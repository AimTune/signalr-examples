namespace backend.Models
{
    public class Group
    {
        public string Name { get; set; } = string.Empty;
        public List<User> Users { get; set; } = new List<User>();
    }
}
