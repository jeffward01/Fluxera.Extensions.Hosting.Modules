﻿namespace Fluxera.Extensions.Hosting.Modules.Persistence
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Fluxera.Extensions.DependencyInjection;
	using Fluxera.Extensions.Hosting.Modules.Caching;
	using Fluxera.Extensions.Hosting.Modules.Configuration;
	using Fluxera.Extensions.Hosting.Modules.DataManagement;
	using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
	using Fluxera.Extensions.Hosting.Modules.Persistence.Contributors;
	using Fluxera.Extensions.Validation.DataAnnotations;
	using Fluxera.Extensions.Validation.FluentValidation;
	using Fluxera.Guards;
	using Fluxera.Repository;
	using Fluxera.Repository.Caching;
	using Fluxera.Utilities.Extensions;
	using JetBrains.Annotations;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.DependencyInjection.Extensions;

	/// <summary>
	///     A module that enabled persistence.
	/// </summary>
	[PublicAPI]
	[DependsOn(typeof(CachingModule))]
	[DependsOn(typeof(DataManagementModule))]
	[DependsOn(typeof(OpenTelemetryModule))]
	public sealed class PersistenceModule : ConfigureApplicationModule
	{
		/// <inheritdoc />
		public override void PreConfigureServices(IServiceConfigurationContext context)
		{
			// Add the configure options contributor.
			context.Services.AddConfigureOptionsContributor<ConfigureOptionsContributor>();

			// Add the tracer provider contributor.
			context.Services.AddTracerProviderContributor<TracerProviderContributor>();

			// Initialize the repository provider contributor list.
			context.Log("AddObjectAccessor(RepositoryProviderContributorList)",
				services => services.AddObjectAccessor(new RepositoryProviderContributorList(), ObjectAccessorLifetime.ConfigureServices));

			// Initialize the repository contributor list.
			context.Log("AddObjectAccessor(RepositoryContributorDictionary)",
				services => services.AddObjectAccessor(new RepositoryContributorDictionary(), ObjectAccessorLifetime.ConfigureServices));

			// Initialize the repository context contributor list.
			context.Log("AddObjectAccessor(RepositoryContextContributorDictionary)",
				services => services.AddObjectAccessor(new RepositoryContextContributorDictionary(), ObjectAccessorLifetime.ConfigureServices));
		}

		/// <inheritdoc />
		public override void PostConfigureServices(IServiceConfigurationContext context)
		{
			// Add database name provider.
			context.Log("AddDatabaseNameProvider", services =>
			{
				services.TryAddTransient<IDatabaseNameProvider, DefaultDatabaseNameProvider>();
			});

			// Add database connection string provider.
			context.Log("AddDatabaseConnectionStringProvider", services =>
			{
				services.TryAddTransient<IDatabaseConnectionStringProvider, DefaultDatabaseConnectionStringProvider>();
			});

			// Get the repository contributors.
			RepositoryContributorDictionary contributorDictionary = context.Services.GetObject<RepositoryContributorDictionary>();

			// Get the repository context contributors.
			RepositoryContextContributorDictionary contributorContextDictionary = context.Services.GetObject<RepositoryContextContributorDictionary>();

			// Get the repository provider contributors.
			RepositoryProviderContributorList contributorList = context.Services.GetObject<RepositoryProviderContributorList>();

			// Get persistence options to use in service configuration.
			PersistenceOptions persistenceOptions = context.Services.GetOptions<PersistenceOptions>();
			persistenceOptions.ConnectionStrings = persistenceOptions.ConnectionStrings;

			// Add default services and the repositories.
			context.Services.AddRepository(builder =>
			{
				foreach((string repositoryName, RepositoryOptions repositoryOptions) in persistenceOptions.Repositories)
				{
					Type contextType = null;
					if(contributorContextDictionary.TryGetValue(repositoryName, out IRepositoryContextContributor repositoryContextContributor))
					{
						contextType = repositoryContextContributor.ConfigureRepositoryContext();
					}

					Guard.Against.Null(contextType, message: $"The repository context must be configured for repository '{repositoryName}'.");

					IList<IRepositoryContributor> repositoryContributors = contributorDictionary.GetOrDefault(repositoryName);
					if(repositoryContributors == null)
					{
						throw new InvalidOperationException($"No repository contributor was added for repository '{repositoryName}'.");
					}

					context.Log($"AddRepository({repositoryName}, {repositoryOptions.ProviderName})", _ =>
					{
						if(string.IsNullOrWhiteSpace(repositoryOptions.ProviderName))
						{
							throw new InvalidOperationException("No repository provider specified.");
						}

						// Select a configured repository provider.
						IRepositoryProviderContributor repositoryProviderContributor = contributorList.FirstOrDefault(x => x.RepositoryProviderName == repositoryOptions.ProviderName);
						if(repositoryProviderContributor == null)
						{
							throw new InvalidOperationException($"No repository provider contributor found for provider '{repositoryOptions.ProviderName}'.");
						}

						// Configure the used provider.
						repositoryProviderContributor.AddRepository(builder, repositoryName, contextType, repositoryOptionsBuilder =>
						{
							// Enable/Disable the UoW feature.
							if(repositoryOptions.EnableUnitOfWork)
							{
								repositoryOptionsBuilder.EnableUnitOfWork();
							}

							// Configure for what aggregate root types this repository uses.
							foreach(IRepositoryContributor persistenceContributor in repositoryContributors)
							{
								IRepositoryAggregatesBuilder repositoryAggregatesBuilder = new RepositoryAggregatesBuilder(repositoryOptionsBuilder);
								persistenceContributor.ConfigureAggregates(repositoryAggregatesBuilder, context);
							}

							// Configure the domain event handlers.
							repositoryOptionsBuilder.AddDomainEventHandling(domainHandlerOptionsBuilder =>
							{
								foreach(IRepositoryContributor repositoryContributor in repositoryContributors)
								{
									IDomainEventHandlersBuilder domainEventHandlersBuilder = new DomainEventHandlersBuilder(domainHandlerOptionsBuilder);
									repositoryContributor.ConfigureDomainEventHandlers(domainEventHandlersBuilder, context);
								}
							});

							// Configure validation providers.
							repositoryOptionsBuilder.AddValidation(validationOptionsBuilder =>
							{
								validationOptionsBuilder.AddValidatorFactory(validationBuilder =>
								{
									validationBuilder.AddDataAnnotations(validationOptionsBuilder.RepositoryName);
									validationBuilder.AddFluentValidation(validationOptionsBuilder.RepositoryName, fluentValidationOptions =>
									{
										foreach(IRepositoryContributor repositoryContributor in repositoryContributors)
										{
											IValidatorsBuilder validatorsBuilder = new ValidatorsBuilder(fluentValidationOptions);
											repositoryContributor.ConfigureValidators(validatorsBuilder, context);
										}
									});
								});
							});

							// Configure the interception.
							repositoryOptionsBuilder.AddInterception(interceptionOptionsBuilder =>
							{
								foreach(IRepositoryContributor repositoryContributor in repositoryContributors)
								{
									IInterceptorsBuilder interceptorsBuilder = new InterceptorsBuilder(interceptionOptionsBuilder);
									repositoryContributor.ConfigureInterceptors(interceptorsBuilder, context);
								}
							});

							// Configure caching.
							repositoryOptionsBuilder.AddCaching(cachingOptionsBuilder =>
							{
								foreach(IRepositoryContributor repositoryContributor in repositoryContributors)
								{
									ICachingBuilder cachingBuilder = new CachingBuilder(cachingOptionsBuilder);
									repositoryContributor.ConfigureCaching(cachingBuilder, context);
								}
							});
						}, context);
					});
				}
			});
		}

		/// <inheritdoc />
		public override void PostConfigure(IApplicationInitializationContext context)
		{
			context.Log("Initialize(CachePrefixManager)", serviceProvider =>
			{
				ICachePrefixManager cachePrefixManager = serviceProvider.GetRequiredService<ICachePrefixManager>();
				CacheManager.CachePrefixManager = cachePrefixManager;
			});
		}
	}
}
