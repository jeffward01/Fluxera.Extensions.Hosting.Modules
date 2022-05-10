﻿namespace Fluxera.Extensions.Hosting.Modules.HttpClient
{
	using Fluxera.Extensions.DependencyInjection;
	using JetBrains.Annotations;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	/// <summary>
	///     Extension methods for the <see cref="IServiceCollection" /> type.
	/// </summary>
	[PublicAPI]
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		///     Adds the given data seeder contributor to the list of contributors.
		/// </summary>
		/// <remarks>
		///     The seeders are only added in a non-production environment.
		/// </remarks>
		/// <typeparam name="TContributor"></typeparam>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddHttpClientServiceContributor<TContributor>(this IServiceCollection services)
			where TContributor : class, IHttpClientServiceContributor, new()
		{
			HttpClientServiceRegistrationContributorList contributorList = services.GetObject<HttpClientServiceRegistrationContributorList>();
			if(contributorList != null)
			{
				TContributor contributor = new TContributor();
				contributorList.Add(contributor);
			}
			else
			{
				ILogger logger = services.GetObjectOrDefault<ILogger>();
				logger?.LogWarning("The contributor list for {Contributor} was not available.", typeof(IHttpClientServiceContributor));
			}

			return services;
		}

		public static IServiceCollection AddHttpClientBuilderContributor<TContributor>(this IServiceCollection services)
			where TContributor : class, IHttpClientBuilderContributor, new()
		{
			HttpClientBuilderContributorList contributorList = services.GetObject<HttpClientBuilderContributorList>();
			if(contributorList != null)
			{
				TContributor contributor = new TContributor();
				contributorList.Add(contributor);
			}
			else
			{
				ILogger logger = services.GetObjectOrDefault<ILogger>();
				logger?.LogWarning("The contributor list for {Contributor} was not available.", typeof(IHttpClientBuilderContributor));
			}

			return services;
		}
	}
}
