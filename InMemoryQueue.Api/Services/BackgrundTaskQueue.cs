using System.Threading.Channels;
using MediatR;

namespace InMemoryQueue.Api.Services;

public interface IBackgroundTaskQueue
{
    Task<INotification> DequeueAsync(CancellationToken stoppingToken);
    Task QueueTaskAsync(INotification task, CancellationToken stoppingToken = default);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<INotification> _queue = Channel.CreateUnbounded<INotification>();

    public async Task QueueTaskAsync(INotification task, CancellationToken stoppingToken = default)
    {
        await _queue.Writer.WriteAsync(task, stoppingToken);
    }

    public async Task<INotification> DequeueAsync(CancellationToken stoppingToken)
    {
        return await _queue.Reader.ReadAsync(stoppingToken);
    }
    
}