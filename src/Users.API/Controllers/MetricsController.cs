using Users.Domain.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Controllers;

/// <summary>
/// Controller para expor métricas e estatísticas do sistema
/// SRP - Responsabilidade única: apenas expor endpoints de métricas
/// </summary>
[Route("api/v1/metrics")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly IPrometheusFormatter _formatter;

    public MetricsController(IMetricsService metricsService, IPrometheusFormatter formatter)
    {
        _metricsService = metricsService;
        _formatter = formatter;
    }

    /// <summary>
    /// Obtém métricas do sistema em formato Prometheus
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetMetrics()
    {
        var metrics = _formatter.FormatMetrics(_metricsService);
        return Content(metrics, "text/plain");
    }

    /// <summary>
    /// Obtém estatísticas do sistema em formato JSON
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            metrics = new
            {
                users = new
                {
                    created = _metricsService.GetUsersCreated(),
                    loggedIn = _metricsService.GetUsersLoggedIn()
                },
                wallets = new
                {
                    queried = _metricsService.GetWalletsQueried(),
                    updated = _metricsService.GetWalletsUpdated(),
                    created = _metricsService.GetWalletsCreated()
                },
                system = new
                {
                    cpuUsage = _metricsService.GetCpuUsage(),
                    memoryUsage = _metricsService.GetMemoryUsage(),
                    uptime = _metricsService.GetUptime()
                }
            }
        });
    }
}

