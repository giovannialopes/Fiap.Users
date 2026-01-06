namespace Users.Domain.Services.Interface;

/// <summary>
/// Interface para serviço de métricas (DIP - Dependency Inversion Principle)
/// </summary>
public interface IMetricsService
{
    void IncrementUsersCreated();
    void IncrementUsersLoggedIn();
    void IncrementWalletsQueried();
    void IncrementWalletsUpdated();
    void IncrementWalletsCreated();
    
    // Getters para exposição de métricas
    long GetUsersCreated();
    long GetUsersLoggedIn();
    long GetWalletsQueried();
    long GetWalletsUpdated();
    long GetWalletsCreated();
    
    // Métricas do sistema
    double GetCpuUsage();
    long GetMemoryUsage();
    TimeSpan GetUptime();
}

