namespace MyFirstApp.Domain.Enums
{
    public enum ExpenseFilter
    {
        PaidExpensesFirst = 0,
        PaidExpensesLast = 1,
        PaidExpensesOnly = 2,
        UnpaidExpensesOnly = 3,
        AlphabeticalOrder = 4,
        ReverseAlphabeticalOrder = 5,
    }
}
