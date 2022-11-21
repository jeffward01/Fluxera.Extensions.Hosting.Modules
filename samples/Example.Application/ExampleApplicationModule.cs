﻿namespace Example.Application
{
	using Example.Application.Contributors;
	using Example.Infrastructure;
	using Fluxera.Extensions.Hosting;
	using Fluxera.Extensions.Hosting.Modules;
	using Fluxera.Extensions.Hosting.Modules.Application;
	using Fluxera.Extensions.Hosting.Modules.AutoMapper;
	using Fluxera.Extensions.Hosting.Modules.Configuration;
	using JetBrains.Annotations;

	[PublicAPI]
	[DependsOn(typeof(ExampleInfrastructureModule))]
	[DependsOn(typeof(AutoMapperModule))]
	[DependsOn(typeof(ApplicationModule))]
	[DependsOn(typeof(ConfigurationModule))]
	public class ExampleApplicationModule : ConfigureServicesModule
	{
		/// <inheritdoc />
		public override void PreConfigureServices(IServiceConfigurationContext context)
		{
			// Add the mapping profile contributor.
			context.Services.AddMappingProfileContributor<MappingProfileContributor>();
		}

		/// <inheritdoc />
		public override void ConfigureServices(IServiceConfigurationContext context)
		{
			// Add the MediatR services.
			context.Services.AddMediatR();
		}
	}
}
