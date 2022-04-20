﻿namespace Fluxera.Extensions.Hosting.Modules.AspNetCore.Swagger
{
	using JetBrains.Annotations;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;

	/// <summary>
	///     Extension methods for the <see cref="IApplicationInitializationContext" /> type.
	/// </summary>
	[PublicAPI]
	public static class ApplicationInitializationContextExtensions
	{
		/// <summary>
		///     Register the Swagger middleware with optional setup action for DI-injected options.
		/// </summary>
		public static IApplicationInitializationContext UseSwagger(this IApplicationInitializationContext context)
		{
			SwaggerOptions options = context.ServiceProvider.GetRequiredService<IOptions<SwaggerOptions>>().Value;
			if(options.Enabled)
			{
				WebApplication app = context.GetApplicationBuilder();
				context.Log("UseSwagger", _ => app.UseSwagger());
			}

			return context;
		}

		/// <summary>
		///     Register the SwaggerUI middleware with optional setup action for DI-injected options.
		/// </summary>
		public static IApplicationInitializationContext UseSwaggerUI(this IApplicationInitializationContext context)
		{
			SwaggerOptions options = context.ServiceProvider.GetRequiredService<IOptions<SwaggerOptions>>().Value;
			if(options.Enabled)
			{
				WebApplication app = context.GetApplicationBuilder();
				context.Log("UseSwaggerUI", _ => app.UseSwaggerUI());
			}

			return context;
		}
	}
}
