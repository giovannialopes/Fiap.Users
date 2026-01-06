using System.Diagnostics;
using Users.Domain.Services.Interface;

namespace Users.Domain.Services.Class;

/// <summary>
/// Serviço de métricas (SRP - Single Responsibility: apenas gerencia métricas)
/// </summary>
public class MetricsService : IMetricsService
{
    private long _usersCreated = 0;
    private long _usersLoggedIn = 0;
    private long _walletsQueried = 0;
    private long _walletsUpdated = 0;
    private long _walletsCreated = 0;
    private static readonly Process _process = Process.GetCurrentProcess();

    public void IncrementUsersCreated() => Interlocked.Increment(ref _usersCreated);
    public void IncrementUsersLoggedIn() => Interlocked.Increment(ref _usersLoggedIn);
    public void IncrementWalletsQueried() => Interlocked.Increment(ref _walletsQueried);
    public void IncrementWalletsUpdated() => Interlocked.Increment(ref _walletsUpdated);
    public void IncrementWalletsCreated() => Interlocked.Increment(ref _walletsCreated);

    public long GetUsersCreated() => _usersCreated;
    public long GetUsersLoggedIn() => _usersLoggedIn;
    public long GetWalletsQueried() => _walletsQueried;
    public long GetWalletsUpdated() => _walletsUpdated;
    public long GetWalletsCreated() => _walletsCreated;

    public double GetCpuUsage() => _process.TotalProcessorTime.TotalSeconds;
    public long GetMemoryUsage() => _process.WorkingSet64;
    public TimeSpan GetUptime() => DateTime.UtcNow - _process.StartTime.ToUniversalTime();
}

