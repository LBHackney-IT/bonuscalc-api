using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using BonusCalcApi.V1.Controllers;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using BonusCalcApi.V1.UseCase.Interfaces;
using BonusCalcApi.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Npgsql;

namespace BonusCalcApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AWSSDKHandler.RegisterXRayForAllServices();
        }

        private IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> ApiVersions { get; set; }
        private const string ApiName = "BonusCalc";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddNewtonsoftJson(o => o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney API Key",
                        Name = "X-Api-Key",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Token" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in ApiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion.ToString()}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.Name);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            ConfigureLogging(services, Configuration);

            ConfigureDbContext(services);

            RegisterGateways(services);
            RegisterUseCases(services);
            RegisterHelpers(services);

            services.AddTransient<IDbSaver, DbSaver>();
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                                ?? Configuration.GetValue<string>("DatabaseConnectionString");

            services.AddDbContext<BonusCalcContext>(options =>
            {
                //var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                //dataSourceBuilder.MapEnum<BandChangeDecision>();
                //var dataSource = dataSourceBuilder.Build();

                options
                    //.UseNpgsql(dataSource)
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention();
            });
        }

        [SuppressMessage("SonarCube", "S4792", Justification = "Reviewed configuration")]
        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            // We rebuild the logging stack so as to ensure the console logger is not used in production.
            // See here: https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down
            services.AddLogging(config =>
            {
                // clear out default configuration
                config.ClearProviders();

                config.AddConfiguration(configuration.GetSection("Logging"));
                config.AddDebug();
                config.AddEventSourceLogger();

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
                {
                    config.AddConsole();
                }
            });
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IBandChangeGateway, BandChangeGateway>();
            services.AddScoped<IBonusPeriodGateway, BonusPeriodGateway>();
            services.AddScoped<IOperativeGateway, OperativeGateway>();
            services.AddScoped<IOperativeProjectionGateway, OperativeProjectionGateway>();
            services.AddScoped<IOutOfHoursSummaryGateway, OutOfHoursSummaryGateway>();
            services.AddScoped<IOvertimeSummaryGateway, OvertimeSummaryGateway>();
            services.AddScoped<ITimesheetGateway, TimesheetGateway>();
            services.AddScoped<IPayElementTypeGateway, PayElementTypeGateway>();
            services.AddScoped<ISchemeGateway, SchemeGateway>();
            services.AddScoped<ISummaryGateway, SummaryGateway>();
            services.AddScoped<IWeekGateway, WeekGateway>();
            services.AddScoped<IWorkElementGateway, WorkElementGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddTransient<ICreateBonusPeriodUseCase, CreateBonusPeriodUseCase>();
            services.AddTransient<ICloseBonusPeriodUseCase, CloseBonusPeriodUseCase>();
            services.AddTransient<IGetBonusPeriodsUseCase, GetBonusPeriodsUseCase>();
            services.AddTransient<IGetBonusPeriodUseCase, GetBonusPeriodUseCase>();
            services.AddTransient<IGetCurrentBonusPeriodsUseCase, GetCurrentBonusPeriodsUseCase>();
            services.AddTransient<IGetBandChangeUseCase, GetBandChangeUseCase>();
            services.AddTransient<IGetBandChangesUseCase, GetBandChangesUseCase>();
            services.AddTransient<IGetBandChangeAuthorisationsUseCase, GetBandChangeAuthorisationsUseCase>();
            services.AddTransient<ISupervisorBandDecisionUseCase, SupervisorBandDecisionUseCase>();
            services.AddTransient<IManagerBandDecisionUseCase, ManagerBandDecisionUseCase>();
            services.AddTransient<IGetBonusPeriodForChangesUseCase, GetBonusPeriodForChangesUseCase>();
            services.AddTransient<IGetOperativeUseCase, GetOperativeUseCase>();
            services.AddTransient<IGetOperativesUseCase, GetOperativesUseCase>();
            services.AddTransient<IGetProjectedChangesUseCase, GetProjectedChangesUseCase>();
            services.AddTransient<IGetOperativeTimesheetUseCase, GetOperativeTimesheetUseCase>();
            services.AddTransient<IGetOperativeSummaryUseCase, GetOperativeSummaryUseCase>();
            services.AddTransient<IGetOutOfHoursSummariesUseCase, GetOutOfHoursSummariesUseCase>();
            services.AddTransient<IGetOvertimeSummariesUseCase, GetOvertimeSummariesUseCase>();
            services.AddTransient<IGetPayElementTypeUseCase, GetPayElementTypeUseCase>();
            services.AddTransient<IGetSchemesUseCase, GetSchemesUseCase>();
            services.AddTransient<IGetWeekUseCase, GetWeekUseCase>();
            services.AddTransient<IGetWorkElementsUseCase, GetWorkElementsUseCase>();
            services.AddTransient<IStartBandChangeProcessUseCase, StartBandChangeProcessUseCase>();
            services.AddTransient<IUpdateBandChangeReportSentAtUseCase, UpdateBandChangeReportSentAtUseCase>();
            services.AddTransient<IUpdateOperativeReportSentAtUseCase, UpdateOperativeReportSentAtUseCase>();
            services.AddTransient<IUpdateWeekReportsSentAtUseCase, UpdateWeekReportsSentAtUseCase>();
            services.AddTransient<IUpdateTimesheetUseCase, UpdateTimesheetUseCase>();
            services.AddTransient<IUpdateWeekUseCase, UpdateWeekUseCase>();
        }

        private static void RegisterHelpers(IServiceCollection services)
        {
            services.AddTransient<IOperativeHelpers, OperativeHelpers>();
            services.AddTransient<IDbSaver, DbSaver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelation();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseXRay("bonuscalc-api");

            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            ApiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in ApiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseRouting();
            app.UseSentryTracing();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
