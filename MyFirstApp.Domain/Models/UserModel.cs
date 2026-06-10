using System.Runtime.CompilerServices;
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

        public IReadOnlyList<string> Validate()
        {
            IList<string> validationErrors = [];

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                validationErrors.Add("You must enter a first name");
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                validationErrors.Add("You must enter a last name");
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                validationErrors.Add("You must enter a username");
            }

            return validationErrors.AsReadOnly();
        }

        public string ToFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}
