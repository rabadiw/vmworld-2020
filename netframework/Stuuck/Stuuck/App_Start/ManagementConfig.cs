using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Common.Diagnostics;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health.Contributor;
using Steeltoe.Management.Exporter.Metrics;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace Stuuck
{
    public static class ManagementConfig
    {
        public static IMetricsExporter MetricsExporter { get; set; }

        public static void ConfigureActuators(
            IConfiguration config,
            ILoggerProvider logger,
            IApiExplorer api,
            ILoggerFactory factory = null)
        {
            ActuatorConfigurator.UseCloudFoundryActuators(config, logger, GetHealthContributors(config), api, factory);
        }

        public static void Start()
        {
            DiagnosticsManager.Instance.Start();
            if (MetricsExporter != null)
            {
                MetricsExporter.Start();
            }
        }

        public static void Stop()
        {
            DiagnosticsManager.Instance.Stop();
            if (MetricsExporter != null)
            {
                MetricsExporter.Stop();
            }
        }

        private static IEnumerable<IHealthContributor> GetHealthContributors(IConfiguration configuration)
        {
            var healthContributors = new List<IHealthContributor>
            {
                new DiskSpaceContributor(),
            };

            return healthContributors;
        }
    }
}