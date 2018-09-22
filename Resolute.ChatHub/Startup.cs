using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Resolute.ChatHub.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Resolute.ChatHub.Models;
using Resolute.ChatHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace Resolute.ChatHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(option => option.AddPolicy("AppPolicy", builder => {
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials();
            }));

            services.AddSignalR();
            services.Configure<Settings>(Options => {
                Options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                Options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });
            services.AddTransient<ISolutionService, SolutionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AppPolicy");
            app.UseSignalR(routes => {
                routes.MapHub<RTMHub>("/chathub");
            });

            app.UseMvc();
        }
    }
}
