using System;
using DevIO.Api.Extentions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(opt =>
            {
                opt.ApiKey = "a5db8d20da67418c9b3e8a4bb41ad7a3";
                opt.LogId = new Guid("c6fdf9b8-fdda-4134-bfde-fb35e2628130");
            });

            #region para o pagar o log feito manualmente - packager Elmah.Io.Extensions.Logging
            //services.AddLogging(builder =>
            //    {
            //        builder.Services.AddElmahIo(opt =>
            //        {
            //            opt.ApiKey = "a5db8d20da67418c9b3e8a4bb41ad7a3";
            //            opt.LogId = new Guid("c6fdf9b8-fdda-4134-bfde-fb35e2628130");
            //        });

            //        builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //    }); 
            #endregion

            services.AddHealthChecks()
                .AddElmahIoPublisher("a5db8d20da67418c9b3e8a4bb41ad7a3", new Guid("c6fdf9b8-fdda-4134-bfde-fb35e2628130"), "API-FORNECEDORES")
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");//monitoramente da saude do banco X package AspNetCore.HealthChecks.SqlServer

//            services.AddHealthChecksUI();//interface package AspNetCore.HealthChecks.UI

            return services;
        }

        public static IApplicationBuilder UseLogginConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            app.UseHealthChecks("/api/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(opt =>
            {
                opt.UIPath = "/api/hc-ui";

            });

            return app;
        }
    }
}
