using Users.Domain.Services.Interface;

namespace Users.Domain.Services.Class;

/// <summary>
/// Formatador de métricas Prometheus (SRP - Single Responsibility: apenas formata)
/// </summary>
public class PrometheusFormatter : IPrometheusFormatter
{
    public string FormatMetrics(IMetricsService metricsService)
    {
        var metrics = new System.Text.StringBuilder();
        
        // DRY - Método auxiliar para evitar duplicação
        AppendCounter(metrics, "users_created_total", "Total users created", metricsService.GetUsersCreated());
        AppendCounter(metrics, "users_logged_in_total", "Total users logged in", metricsService.GetUsersLoggedIn());
        AppendCounter(metrics, "wallets_queried_total", "Total wallets queried", metricsService.GetWalletsQueried());
        AppendCounter(metrics, "wallets_updated_total", "Total wallets updated", metricsService.GetWalletsUpdated());
        AppendCounter(metrics, "wallets_created_total", "Total wallets created", metricsService.GetWalletsCreated());
        
        // Métricas do sistema
        AppendCounter(metrics, "process_cpu_seconds_total", "Total CPU time used", metricsService.GetCpuUsage());
        AppendGauge(metrics, "process_memory_bytes", "Memory usage in bytes", metricsService.GetMemoryUsage());

        return metrics.ToString();
    }

    // DRY - Evita duplicação de código
    private static void AppendCounter(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} counter");
        sb.AppendLine($"{name} {value}");
    }

    private static void AppendGauge(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} gauge");
        sb.AppendLine($"{name} {value}");
    }
}

