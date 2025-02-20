﻿namespace Fluxera.Extensions.Hosting.Modules.Messaging.AzureServiceBus
{
	using Fluxera.Extensions.Hosting.Modules.Configuration;
	using Fluxera.Extensions.Hosting.Modules.Messaging.AzureServiceBus.Contributors;
	using JetBrains.Annotations;

	/// <summary>
	///     A module that enables Azure ServiceBus messaging.
	/// </summary>
	[PublicAPI]
	[DependsOn(typeof(MessagingModule))]
	public sealed class AzureServiceBusMessagingModule : ConfigureServicesModule
	{
		/// <inheritdoc />
		public override void PreConfigureServices(IServiceConfigurationContext context)
		{
			// Add the configure options contributor.
			context.Services.AddConfigureOptionsContributor<ConfigureOptionsContributor>();

			// Add the transport contributor.
			context.Services.AddTransportContributor<TransportContributor>();
		}
	}
}
