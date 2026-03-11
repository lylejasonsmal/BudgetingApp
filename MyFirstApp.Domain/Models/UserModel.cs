using SQLite;

namespace MyFirstApp.Domain.Models
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public DateTime? Birthday { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? Description { get; set; }
    }
}
