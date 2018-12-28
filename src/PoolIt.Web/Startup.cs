namespace PoolIt.Web
{
    using System;
    using System.IO.Abstractions;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using AutoMapper;
    using Data;
    using Data.Common;
    using Data.Repository;
    using Extensions;
    using Infrastructure.Mapping;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Middlewares.MiddlewareServices;
    using Middlewares.MiddlewareServices.Contracts;
    using Newtonsoft.Json.Linq;
    using PoolIt.Models;
    using Services;
    using Services.Contracts;
    using Services.Helpers;
    using SignalRHubs;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookieTempDataProviderOptions>(options => { options.Cookie.IsEssential = true; });

            services.AddDbContext<PoolItDbContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<PoolItUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 3;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 0;
                })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<PoolItDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Error403";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddAuthentication()
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = this.Configuration["GitHub:ClientId"];
                    options.ClientSecret = this.Configuration["GitHub:ClientSecret"];
                    options.CallbackPath = new PathString("/login-github");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request =
                                new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization =
                                new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user);
                        }
                    };
                });

            services.AddMvc(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSignalR();

            services.AddResponseCompression(opt => opt.EnableForHttps = true);


            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddScoped<IManufacturersService, ManufacturersService>();
            services.AddScoped<IModelsService, ModelsService>();
            services.AddScoped<ICarsService, CarsService>();
            services.AddScoped<IRidesService, RidesService>();
            services.AddScoped<IJoinRequestsService, JoinRequestsService>();
            services.AddScoped<IInvitationsService, InvitationsService>();
            services.AddScoped<IConversationsService, ConversationsService>();
            services.AddScoped<IPersonalDataService, PersonalDataService>();
            services.AddScoped<IContactMessagesService, ContactMessagesService>();

            services.AddScoped<IRandomStringGeneratorHelper, RandomStringGeneratorHelper>();

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<ILocationHelper, LocationHelper>();
            services.AddSingleton<IRateLimitingService, RateLimitingService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());

            app.EnsureAdminRoleCreatedAsync().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();

            app.UseStatusCodePages();
            app.UseStatusCodePagesWithReExecute("/Error{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes => routes.MapHub<ConversationsHub>("/conversationsHub"));

            app.UseRateLimiting();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}