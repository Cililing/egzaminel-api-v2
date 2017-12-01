using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EgzaminelAPI.Models;
using EgzaminelAPI.DataAccess;
using EgzaminelAPI.Context;
using Microsoft.AspNetCore.Authorization;
using EgzaminelAPI.Auth;
using Microsoft.AspNetCore.Http;

namespace EgzaminelAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TokenValidation", policy => policy.Requirements.Add(new TokenRequirement()));
            });

            services.AddScoped<IRepo, Repo>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEgzaminelContext, EgzaminelContext>();
            services.AddScoped<IGroupsContext, GroupsContext>();
            services.AddScoped<IEventsContext, EventsContext>();
            services.AddScoped<IUsersContext, UsersContext>();
            services.AddScoped<IMapper, Mapper>();
            services.AddScoped<IAuthorizationHandler, TokenHandler>(); // every auth handler add as <IAuthorizationHandler, NewHandler>


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfig, Config>();

            var respositoryInstance = services.BuildServiceProvider().GetService<IRepo>();

            // register contexts
            services.Add(new ServiceDescriptor(typeof(EgzaminelContext),
                new EgzaminelContext(new Config(Configuration))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
