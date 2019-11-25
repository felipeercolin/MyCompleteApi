using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region Versionamento
            services.AddApiVersioning(opt =>
               {
                   opt.AssumeDefaultVersionWhenUnspecified = true;
                   opt.DefaultApiVersion = new ApiVersion(1, 0);
                   opt.ReportApiVersions = true;
               });

            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            }); 
            #endregion

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                //Suprimindo a validação da feita automaticamente pelo asp net core web api
                //Nao é a validação do ModelState.IsValid.
                opt.SuppressModelStateInvalidFilter = true;

            });

            #region Cors
            //services.AddCors(opt =>
            //   {
            //       opt.AddPolicy("Development", builder =>
            //           builder.AllowAnyOrigin()
            //               .AllowAnyMethod()
            //               .AllowAnyHeader()
            //               .AllowCredentials());

            //       opt.AddPolicy("Production", builder =>
            //           builder.WithMethods("GET")
            //               .WithOrigins("http://desenvolvedor.io")
            //               .SetIsOriginAllowedToAllowWildcardSubdomains()
            //               .WithHeaders(HeaderNames.ContentType, "x-custom-header")
            //               .AllowAnyHeader());
            //   }); 
            #endregion

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseMvc();
            //app.UseCors("Development");

            return app;
        }
    }
}
