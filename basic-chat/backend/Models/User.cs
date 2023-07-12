namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ConnectionID { get; set; } = string.Empty;
    }
}
