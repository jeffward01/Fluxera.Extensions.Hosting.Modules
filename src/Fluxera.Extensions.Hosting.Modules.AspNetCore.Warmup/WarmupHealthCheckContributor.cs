﻿namespace Fluxera.Extensions.Hosting.Modules.AspNetCore.Warmup
{
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;

	internal sealed class WarmupHealthCheckContributor : IHealthCheckContributor
	{
		/// <inheritdoc />
		public HealthCheckDescriptor CreateHealthCheck()
		{
			return new HealthCheckDescriptor(typeof(WarmupHealthCheck), "Warmup", HealthCheckCategory.Ready);
		}
	}
}
