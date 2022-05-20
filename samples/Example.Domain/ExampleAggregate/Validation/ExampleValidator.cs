﻿namespace Example.Domain.ExampleAggregate.Validation
{
	using Example.Domain.ExampleAggregate.Model;
	using FluentValidation;
	using JetBrains.Annotations;

	/// <summary>
	///     A validator that validates example instances.
	/// </summary>
	[UsedImplicitly]
	public sealed class ExampleValidator : AbstractValidator<Example>
	{
		public ExampleValidator()
		{
			this.RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(100);
		}
	}
}
