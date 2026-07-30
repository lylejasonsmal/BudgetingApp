namespace MyFirstApp.Domain.Models
{
    public record CalendarEventModel
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public DateTime StartsAt { get; init; }
        public DateTime EndsAt { get; init; }
    }
}
