using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using OstoslistaAPI.Hubs;
using OstoslistaData;
using Swashbuckle.AspNetCore.Swagger;

namespace OstoslistaAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string API_TITLE = "Shopping List API";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnv"></param>
        public Startup(IHostingEnvironment hostEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnv.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{hostEnv.EnvironmentName}.json", true)
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();

            if (hostEnv.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(true);
            }

            Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                }).AddCookie();

            // Add framework services.
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddTransient<IShoppingListDataService, ShoppingListDataService>();
            services.AddTransient<IShoppingListService, ShoppingListService>();

            string xmlComments = GetXmlCommentsPath();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ui",
                    new Info
                    {
                        Title = API_TITLE,
                        Description = "An API for creating and managing shopping list",
                        Version = "v1",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Email = "ostoslista@ostoslistaapi.azurewebsites.net",
                            Name = "Mikko Andersson",
                            Url = "https://ostoslistaapi.azurewebsites.net/"
                        }
                    });

                if (System.IO.File.Exists(xmlComments))
                {
                    c.IncludeXmlComments(xmlComments);
                }

                c.DescribeAllEnumsAsStrings();
            });

            services.AddDbContext<ShoppingListDataService>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.GetConnectionString("ShoppingListDatabase")));

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".ShoppingList.Session";
                options.IdleTimeout = TimeSpan.FromDays(365);
            });

            services.AddSignalR();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ShoppingListHub>("/shoppingListHub");
            });
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ui/swagger.json", API_TITLE);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(app.ApplicationBasePath, $"{app.ApplicationName}.xml");
        }
    }
}
