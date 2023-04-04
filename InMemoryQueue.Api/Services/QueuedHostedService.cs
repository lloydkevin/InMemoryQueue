using MediatR;

namespace InMemoryQueue.Api.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
/// Using MediatR to dispatch tasks
/// </summary>
public sealed class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ILogger<QueuedHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public QueuedHostedService(ILogger<QueuedHostedService> logger, IServiceScopeFactory scopeFactory, IBackgroundTaskQueue queue)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _queue = queue;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(QueuedHostedService)} is running.");
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Waiting for new queue message");
                var backgroundTask = await _queue.DequeueAsync(stoppingToken);

                _logger.LogInformation("Found task {TaskType}", backgroundTask.GetType());
                using var scope = _scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                _logger.LogInformation("Running task {TaskType}", backgroundTask.GetType());
                await publisher.Publish(backgroundTask, stoppingToken);
                _logger.LogInformation("Completed task {TaskType}", backgroundTask.GetType());
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");
        await base.StopAsync(stoppingToken);
    }
}