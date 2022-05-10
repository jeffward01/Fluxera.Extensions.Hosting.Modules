﻿namespace Fluxera.Extensions.Hosting.Modules.AspNetCore.HttpApi.OData
{
	using Asp.Versioning.OData;
	using Fluxera.Extensions.DependencyInjection;
	using JetBrains.Annotations;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.DependencyInjection.Extensions;
	using Microsoft.Extensions.Logging;

	/// <summary>
	///     Extension methods for the <see cref="IServiceCollection" /> type.
	/// </summary>
	[PublicAPI]
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEdmModelContributor<TContributor>(this IServiceCollection services)
			where TContributor : class, IEdmModelContributor, new()
		{
			TContributor contributor = new TContributor();

			EdmModelContributorList contributorList = services.GetObjectOrDefault<EdmModelContributorList>();
			if(contributorList != null)
			{
				contributorList.Add(contributor);
				services.TryAddSingleton<IModelConfiguration>(contributor);
			}
			else
			{
				ILogger logger = services.GetObjectOrDefault<ILogger>();
				logger?.LogWarning("The contributor list for {Contributor} was not available.", typeof(IEdmModelContributor));
			}

			return services;
		}
	}
}
