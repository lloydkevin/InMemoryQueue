using MediatR;

namespace InMemoryQueue.Api.Services;

public record LongJob(Guid JobId) : INotification;

public class Handler : INotificationHandler<LongJob>
{
    private readonly StatusService _statusService;
    private readonly ILogger<Handler> _logger;

    public Handler(StatusService statusService, ILogger<Handler> logger)
    {
        _statusService = statusService;
        _logger = logger;
    }

    public async Task Handle(LongJob notification, CancellationToken cancellationToken)
    {
        {
            double complete = (i / 20.0 * 100.0);
            _logger.LogInformation("Percentage complete: {Complete} for Job: {JobId}", complete, notification.JobId);
            _statusService.SetStatus(notification.JobId, complete);

            await Task.Delay(1_000, cancellationToken);
        }
    }
}