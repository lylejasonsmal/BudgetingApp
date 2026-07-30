using System.Globalization;
using Microsoft.Maui.ApplicationModel;
using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces;

namespace MyFirstApp.Services.Implementations.GoogleCalendarService
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private const string TemplateBaseUrl = "https://calendar.google.com/calendar/render";

        // Google's web template expects basic-format UTC: YYYYMMDDTHHMMSSZ.
        private const string GoogleUtcFormat = "yyyyMMdd'T'HHmmss'Z'";

        private readonly ILauncher _launcher;

        public GoogleCalendarService(ILauncher launcher)
        {
            _launcher = launcher;
        }

        public Uri BuildEventUri(CalendarEventModel calendarEvent)
        {
            var dates = $"{ToGoogleUtc(calendarEvent.StartsAt)}/{ToGoogleUtc(calendarEvent.EndsAt)}";

            var query = string.Join("&",
                "action=TEMPLATE",
                $"text={Uri.EscapeDataString(calendarEvent.Title ?? string.Empty)}",
                $"dates={dates}",
                $"details={Uri.EscapeDataString(calendarEvent.Description ?? string.Empty)}");

            return new Uri($"{TemplateBaseUrl}?{query}");
        }

        public async Task<Result> AddEventToCalendarAsync(CalendarEventModel calendarEvent)
        {
            var validation = Validate(calendarEvent);
            if (validation.Unsuccessful)
            {
                return validation;
            }

            var launched = await _launcher.OpenAsync(BuildEventUri(calendarEvent));

            return launched
                ? new Result(ResultOutcome.Success, null, "Opening Google Calendar...")
                : new Result(ResultOutcome.Failure, ["Could not open a browser to reach Google Calendar."]);
        }

        private static Result Validate(CalendarEventModel calendarEvent)
        {
            var builder = Result.Builder();

            if (string.IsNullOrWhiteSpace(calendarEvent.Title))
            {
                builder.WithError("Event title cannot be empty.");
            }

            if (calendarEvent.EndsAt <= calendarEvent.StartsAt)
            {
                builder.WithError("Event end must be after the start.");
            }

            return builder.Create();
        }

        // ToUniversalTime treats Local/Unspecified kinds as the device's local time, which is what the app stores.
        private static string ToGoogleUtc(DateTime value) =>
            value.ToUniversalTime().ToString(GoogleUtcFormat, CultureInfo.InvariantCulture);
    }
}
