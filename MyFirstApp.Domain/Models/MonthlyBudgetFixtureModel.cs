using SQLite;

namespace MyFirstApp.Domain.Models
{
    public class MonthlyBudgetFixtureModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public int? NumberOfExpenses { get; set; } = 0;
        public double? StoredNetSalary { get; set; } = 0;
        public double? BudgetedForAmount { get; set; } = 0;
        public double? LeftOverAmount { get; set; } = 0;
        public bool CurrentlyInUse { get; set; }
    }
}