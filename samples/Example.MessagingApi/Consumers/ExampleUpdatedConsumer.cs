﻿namespace Example.MessagingApi.Consumers
{
	using System;
	using System.Threading.Tasks;
	using Example.Domain.Shared.Example.Events;
	using JetBrains.Annotations;
	using MassTransit;

	/// <summary>
	///     A consumer implementation that consumes <see cref="ExampleUpdated" /> event messages.
	/// </summary>
	[UsedImplicitly]
	public sealed class ExampleUpdatedConsumer : IConsumer<ExampleUpdated>
	{
		/// <inheritdoc />
		public Task Consume(ConsumeContext<ExampleUpdated> context)
		{
			Console.WriteLine("CONSUMED EXAMPLE UPDATED");

			return Task.CompletedTask;
		}
	}
}
