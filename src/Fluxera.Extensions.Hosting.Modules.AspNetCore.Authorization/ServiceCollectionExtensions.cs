﻿namespace Fluxera.Extensions.Hosting.Modules.AspNetCore.Authorization
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
		///     Adds the authorize contributor to the list of contributors.
		/// </summary>
		/// <typeparam name="TContributor"></typeparam>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddAuthorizeContributor<TContributor>(this IServiceCollection services)
			where TContributor : class, IAuthorizeContributor, new()
		{
			AuthorizeContributorList contributorList = services.GetObjectOrDefault<AuthorizeContributorList>();
			if(contributorList != null)
			{
				TContributor contributor = new TContributor();
				contributorList.Add(contributor);
			}
			else
			{
				ILogger logger = services.GetObjectOrDefault<ILogger>();
				logger?.LogWarning("The contributor list for {Contributor} was not available.", typeof(IAuthorizeContributor));
			}

			return services;
		}

		public static IServiceCollection AddPolicyContributor<TContributor>(this IServiceCollection services)
			where TContributor : class, IPolicyContributor, new()
		{
			PolicyContributorList contributorList = services.GetObjectOrDefault<PolicyContributorList>();
			if(contributorList != null)
			{
				TContributor contributor = new TContributor();
				contributorList.Add(contributor);
			}
			else
			{
				ILogger logger = services.GetObjectOrDefault<ILogger>();
				logger?.LogWarning("The contributor list for {Contributor} was not available.", typeof(IPolicyContributor));
			}

			return services;
		}
	}
}
