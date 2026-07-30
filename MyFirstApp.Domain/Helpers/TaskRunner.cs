using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyFirstApp.Domain.Helpers;

public static class TaskRunner
{

    private static readonly ConcurrentDictionary<string, ConcurrentQueue<TimeSpan>> MethodCallsAndElapsedTime = new();
    private static readonly ConcurrentDictionary<Exception, string> Exceptions = [];

    public static Func<string, Task>? OnErrorAsync { get; set; }

    public static async Task ExecuteAsync(
        string operation,
        Func<Task> action)
    {
        await ExecuteAsync<object?>(
            operation,
            async () =>
            {
                await action();
                return null;
            });
    }

    public static async Task<T> ExecuteAsync<T>(
        string operation,
        Func<Task<T>> action)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await action();

            Logger.Log($"[{DateTime.Now:HH:mm:ss}] {operation} completed in {stopwatch.Elapsed.TotalMilliseconds:F4}ms");
            MethodCallsAndElapsedTime.AddOrUpdate(operation, _ => new ConcurrentQueue<TimeSpan>([stopwatch.Elapsed]),
                (_, queue) => {
                    queue.Enqueue(stopwatch.Elapsed);
                    return queue;
                });
            return result;
        }
        catch (Exception ex)
        {
            Logger.Log($"[{DateTime.Now:HH:mm:ss}] {operation} failed after {stopwatch.Elapsed.TotalMilliseconds:F4}ms");
            Logger.Log(ex.ToString());
            Exceptions.TryAdd(ex, operation);
            if (OnErrorAsync is null) throw;

            await OnErrorAsync($"{operation} failed. Please try again.");
            return default!;
        }
    }

    public static ConcurrentDictionary<string, ConcurrentQueue<TimeSpan>> GetNerdyStats()
    {
        return MethodCallsAndElapsedTime;
    }

    public static ConcurrentDictionary<Exception, string> GetExceptionsThrown()
    {
        return Exceptions;
    }
}