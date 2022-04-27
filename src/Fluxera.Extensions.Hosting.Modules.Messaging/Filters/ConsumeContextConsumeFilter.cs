﻿namespace Fluxera.Extensions.Hosting.Modules.Messaging.Filters
{
	using System;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using MassTransit;
	using Microsoft.Extensions.Logging;

	[PublicAPI]
	public sealed class ConsumeContextConsumeFilter<T> : IFilter<ConsumeContext<T>>
		where T : class
	{
		private readonly IConsumeContextAccessor consumeContextAccessor;
		private readonly ILogger logger;

		public ConsumeContextConsumeFilter(
			ILoggerFactory loggerFactory,
			IConsumeContextAccessor consumeContextAccessor)
		{
			this.logger = loggerFactory.CreateLogger(this.GetType());
			this.consumeContextAccessor = consumeContextAccessor;
		}

		/// <inheritdoc />
		public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
		{
			try
			{
				// Executed before the message is consumed.
				this.consumeContextAccessor.ConsumeContext = context;

				// Here the next filter in the pipe is called.
				await next.Send(context);

				// Executed after the message was consumed.
				this.consumeContextAccessor.ConsumeContext = null;
			}
			catch(Exception ex)
			{
				this.logger.LogError($"Setting the consume context failed: {ex.Message}");

				// Propagate the exception up the call stack.
				throw;
			}
		}

		/// <inheritdoc />
		public void Probe(ProbeContext context)
		{
		}
	}
}
