using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;

namespace MyFirstApp.Services.Interfaces
{
    public interface IGoogleCalendarService
    {
        Uri BuildEventUri(CalendarEventModel calendarEvent);
        Task<Result> AddEventToCalendarAsync(CalendarEventModel calendarEvent);
    }
}
