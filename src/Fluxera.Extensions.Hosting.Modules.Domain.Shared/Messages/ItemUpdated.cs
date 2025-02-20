﻿namespace Fluxera.Extensions.Hosting.Modules.Domain.Shared.Messages
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using JetBrains.Annotations;
	using MassTransit;

	/// <summary>
	///     A base class for event messages that notifies that an item was updated.
	/// </summary>
	[PublicAPI]
	[Serializable]
	[ExcludeFromTopology]
	public abstract class ItemUpdated : IEvent
	{
		/// <summary>
		///     Gets or sets the events item updated metadata.
		/// </summary>
		[Required]
		public ItemUpdatedData Metadata { get; set; }
	}
}
