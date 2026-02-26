using SQLite;

namespace MyFirstApp.Domain.Models
{
    public class MonthlyBudgetFixtureModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Month { get; set; }

        public string? Year { get; set; }

        public int? NumberOfExpenses { get; set; }

        public double? StoredNetSalary { get; set; }

        public double? BudgetedForAmount { get; set; }

        public double? LeftOverAmount { get; set; }

        public bool CurrentlyInUse { get; set; }
    }
}