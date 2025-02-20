﻿namespace Fluxera.Extensions.Hosting.Modules.Messaging.TransactionalOutbox.Contributors
{
	using System.Diagnostics;
	using Fluxera.Extensions.Hosting.Modules.Configuration;
	using MassTransit;
	using Microsoft.EntityFrameworkCore;

	internal sealed class OutboxContributor<TContext> : IOutboxContributor
		where TContext : DbContext
	{
		/// <inheritdoc />
		public void ConfigureOutbox(IBusRegistrationConfigurator configurator, IServiceConfigurationContext context)
		{
			TransactionalOutboxModuleOptions options = context.Services.GetOptions<TransactionalOutboxModuleOptions>();

			if(options.UseInMemoryInboxOutbox)
			{
				configurator.AddInMemoryInboxOutbox();
			}
			else
			{
				configurator.AddEntityFrameworkOutbox<TContext>(cfg =>
				{
					if(options.Outbox.QueryDelay.HasValue)
					{
						cfg.QueryDelay = options.Outbox.QueryDelay.Value;
					}

					if(options.Outbox.QueryTimeout.HasValue)
					{
						cfg.QueryTimeout = options.Outbox.QueryTimeout.Value;
					}

					if(options.Outbox.QueryMessageLimit.HasValue)
					{
						cfg.QueryMessageLimit = options.Outbox.QueryMessageLimit.Value;
					}

					if(options.Outbox.DuplicateDetectionWindow.HasValue)
					{
						cfg.DuplicateDetectionWindow = options.Outbox.DuplicateDetectionWindow.Value;
					}

					if(options.Outbox.IsolationLevel.HasValue)
					{
						cfg.IsolationLevel = options.Outbox.IsolationLevel.Value;
					}

					switch(options.Outbox.LockStatementProvider)
					{
						case LockStatementProvider.SqlServer:
							cfg.UseSqlServer();
							break;
						case LockStatementProvider.MySql:
							cfg.UseMySql();
							break;
						case LockStatementProvider.Postgres:
							cfg.UsePostgres();
							break;
						default:
							throw new UnreachableException($"Undefined lock statement provider: '{(int)options.Outbox.LockStatementProvider}'.");
					}

					if(options.BusOutbox.UseBusOutbox)
					{
						cfg.UseBusOutbox(config =>
						{
							if(options.BusOutbox.MessageDeliveryLimit.HasValue)
							{
								config.MessageDeliveryLimit = options.BusOutbox.MessageDeliveryLimit.Value;
							}

							if(options.BusOutbox.MessageDeliveryTimeout.HasValue)
							{
								config.MessageDeliveryTimeout = options.BusOutbox.MessageDeliveryTimeout.Value;
							}
						});
					}
				});
			}
		}
	}
}
