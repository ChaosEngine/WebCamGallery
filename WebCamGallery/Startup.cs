using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebCamGallery
{
	public class Startup
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
		}

		private void HandleImagesRequest(IApplicationBuilder app)
		{
			var imageDirectory = Configuration["ImageDirectory"];

			app.Run(async context =>
			{
				if (context.Request.Path.HasValue)
				{
					var name = context.Request.Path.Value.Substring(1);
					if (!string.IsNullOrWhiteSpace(name))
					{
						if (Directory.Exists(imageDirectory))
						{
							var path = Path.Combine(imageDirectory, name);
							if (File.Exists(path))
							{
								await context.Response.SendFileAsync(path);
								return;
							}
						}
					}
				}

				await context.Response.WriteAsync("bad image");
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStaticFiles();

			app.UseMvc();

			//app.Map("/images", HandleImagesRequest);
		}
	}
}
