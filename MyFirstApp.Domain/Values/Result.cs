namespace MyFirstApp.Domain.Values
{
    public record Result
    {
        public bool Successful { get; init; }
        public bool Unsuccessful { get; init; }
        public IList<string>? Errors { get; init; }
        public string? Message { get; init; }

        public Result(ResultOutcome resultOutcome, IList<string>? errors = null, string? message = null)
        {
            if (resultOutcome == ResultOutcome.Success)
            {
                Successful = true;
                Unsuccessful = false;
                Message = message;
            }
            else if (resultOutcome == ResultOutcome.Failure)
            {
                Successful = false;
                Unsuccessful = true;
                Errors = errors;
                Message = message;
            }
        }

        public static ResultBuilder Builder()
        {
            return new ResultBuilder();
        }
    }

    public class ResultBuilder
    {
        private readonly List<string> _errors = new();
        private string? _message; 

        public ResultBuilder WithError(string error)
        {
            _errors.Add(error);
            return this;
        }

        public ResultBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public Result Create()
        {
            if (_errors.Any())
            {
                return new Result(ResultOutcome.Failure, _errors);
            }

            return new Result(ResultOutcome.Success, null, _message);
        }
    }

    public enum ResultOutcome
    {
        Success = 0,
        Failure = 1
    }
}