using SQLite;

namespace MyFirstApp.Domain.Models
{
    public class SavingsPocketModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public double GoalAmount { get; set; } = 0;
        public double CurrentlySavedAmount { get; set; } = 0;

        public bool TryValidate(out string? error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error = "Savings pocket name cannot be empty.";
                return false;
            }

            error = null;
            return true;
        }
    }
}