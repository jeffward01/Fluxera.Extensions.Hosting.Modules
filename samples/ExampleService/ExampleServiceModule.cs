﻿namespace ExampleService
{
	using System.Net;
	using ExampleComponent;
	using Fluxera.Extensions.Hosting;
	using Fluxera.Extensions.Hosting.Modules;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.HttpApi;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.Warmup;
	using Fluxera.Extensions.Validation;
	using JetBrains.Annotations;
	using MadEyeMatt.AspNetCore.ProblemDetails;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	[PublicAPI]
	[DependsOn<WarmupModule>]
	[DependsOn<ExampleComponent>]
	public sealed class ExampleServiceModule : ConfigureApplicationModule
	{
		/// <inheritdoc />
		public override void ConfigureServices(IServiceConfigurationContext context)
		{
			// Configure default exception mappings.
			context.Services.Configure<ProblemDetailsOptions>(options =>
			{
				options.MapStatusCode<ValidationException>(HttpStatusCode.BadRequest);
			});
		}

		/// <inheritdoc />
		public override void Configure(IApplicationInitializationContext context)
		{
			// Configure the HTTP request pipeline.
			if(context.Environment.IsDevelopment())
			{
				context.UseSwaggerUI();
			}

			context.UseHsts();

			context.UseHttpsRedirection();

			context.UseRouting();

			context.UseEndpoints();
		}
	}
}
