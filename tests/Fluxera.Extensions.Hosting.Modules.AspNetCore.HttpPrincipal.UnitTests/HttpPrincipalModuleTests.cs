﻿namespace Fluxera.Extensions.Hosting.Modules.AspNetCore.HttpPrincipal.UnitTests
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Security.Principal;
	using FluentAssertions;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.HttpPrincipal.Extensions;
	using Fluxera.Extensions.Hosting.Modules.Principal;
	using Fluxera.Extensions.Hosting.Modules.UnitTesting;
	using Microsoft.Extensions.DependencyInjection;
	using NUnit.Framework;

	[TestFixture]
	public class HttpPrincipalModuleTests : StartupModuleTestBase<TestModule>
	{
		[SetUp]
		public void SetUp()
		{
			this.StartApplication();
		}

		[TearDown]
		public void TearDown()
		{
			this.StopApplication();
		}

		[Test]
		public void ShouldAddPrincipalAccessor()
		{
			IPrincipalAccessor principalAccessor = this.ApplicationLoader.ServiceProvider.GetRequiredService<IPrincipalAccessor>();
			principalAccessor.Should().NotBeNull();
		}

		[Test]
		public void ShouldAddPrincipalFactory()
		{
			IPrincipal principal = this.ApplicationLoader.ServiceProvider.GetRequiredService<IPrincipal>();
			principal.Should().NotBeNull();
		}

		[Test]
		public void ShouldUseThreadBasedClaimsPrincipalProvider()
		{
			ClaimsPrincipal principal = this.ApplicationLoader.ServiceProvider.GetRequiredService<ClaimsPrincipal>();
			principal.Should().NotBeNull();
		}

		[Test]
		public void ShouldUseThreadBasedPrincipalProvider()
		{
			IEnumerable<IPrincipalProvider> principalProviders = this.ApplicationLoader.ServiceProvider.GetServices<IPrincipalProvider>();
			principalProviders.Should().NotBeNull();
			principalProviders.Count().Should().Be(2);
			principalProviders.Last().Should().BeOfType<HttpContextPrincipalProvider>();
			principalProviders.Last().Position.Should().Be(0);
		}
	}
}
