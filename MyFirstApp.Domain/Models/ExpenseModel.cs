using SQLite;

namespace MyFirstApp.Domain.Models
{
    public class ExpenseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? ExpenseName { get; set; }
        public double? BudgetedForAmount { get; set; } = 0;
        public double? ActualAmount { get; set; } = 0;
        public bool IsPaidFor { get; set; }
        public bool IsARecurringExpense { get; set; }
        public int MonthlyBudgetFixtureId { get; set; }
        public string? Notes { get; set; }
    }
}