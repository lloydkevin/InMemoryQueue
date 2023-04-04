using System.Collections.Concurrent;

namespace InMemoryQueue.Api.Services;

public class StatusService
{
    private readonly Dictionary<Guid, double> _jobs = new();
    public void SetStatus(Guid id, double percentComplete)
    {
        _jobs[id] = percentComplete;
        if (percentComplete >= 100)
            _jobs.Remove(id);
    }

    public double GetStatus(Guid id)
    {
        return _jobs[id];
    }
}