namespace Fluxera.Extensions.Hosting.Modules.Persistence.UnitTests
{
	using Fluxera.Extensions.Hosting.Modules.Persistence.InMemory;

	[DependsOn(typeof(InMemoryPersistenceModule))]
	public class TestModule : ConfigureServicesModule
	{
		/// <inheritdoc />
		public override void PreConfigureServices(IServiceConfigurationContext context)
		{
			context.Services.AddRepositoryContributor<RepositoryContributor>("Test");
		}
	}
}
