namespace MyFirstApp.Domain.Values
{
    public record Result
    {
        public bool Successful { get; init; }
        public bool Unsuccessful { get; init; }
        public string? Error { get; init; }

        public Result(ResultOutcome resultOutcome, string? errorMessage = null)
        {
            if (resultOutcome == ResultOutcome.Success)
            {
                Successful = true;
                Unsuccessful = false;
            }
            else if(resultOutcome == ResultOutcome.Failure)
            {
                Successful = false;
                Unsuccessful = true;
                Error = errorMessage;
            }
        }
    }

    public enum ResultOutcome
    {
        Success = 0,
        Failure = 1
    }
}
