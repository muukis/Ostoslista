﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using F23.StringSimilarity;
using F23.StringSimilarity.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using OstoslistaAPI.Common;
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
        public Startup(IWebHostEnvironment hostEnv)
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
                //builder.AddApplicationInsightsSettings(true);
            }

            Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            //services.AddApplicationInsightsTelemetry(Configuration);

            services.AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    googleOptions.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                    googleOptions.ClaimActions.Clear();
                    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                    googleOptions.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    googleOptions.ClaimActions.MapJsonKey("picture", "picture");
                }).AddCookie();

            services.AddRazorPages();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddTransient<IShoppingListDataService, ShoppingListDataService>();
            services.AddTransient<IShoppingListService, ShoppingListService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            string xmlComments = GetXmlCommentsPath();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ui",
                    new OpenApiInfo
                    {
                        Title = API_TITLE,
                        Description = "An API for creating and managing shopping list",
                        Version = "v1",
                        TermsOfService = null,
                        Contact = new OpenApiContact
                        {
                            Email = "ostoslistat@ostoslistatapi.azurewebsites.net",
                            Name = "Mikko Andersson",
                            Url = new Uri("https://ostoslistatapi.azurewebsites.net/")
                        }
                    });

                c.OperationFilter<ApiOperationFilters>();

                if (System.IO.File.Exists(xmlComments))
                {
                    c.IncludeXmlComments(xmlComments);
                }

                //c.DescribeAllEnumsAsStrings();
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ApiHttpContext.Configure(
                app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
            );

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<ShoppingListHub>("/shoppingListHub");
            }); 

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
