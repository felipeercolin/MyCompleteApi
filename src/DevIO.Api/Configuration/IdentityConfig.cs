using System.Text;
using DevIO.Api.Data;
using DevIO.Api.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DevIO.Api.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityCore<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddErrorDescriber<IdentityMessagesPtBr>()
                    .AddDefaultTokenProviders();

            #region JWT
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    //Validar se o emissor do token é o mesmo da aplicacao, validando tbm pela chave que foi passada além do nome do emissor
                    ValidateIssuerSigningKey =  true,
                    //Chave para Validacao
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //Validar somente o nome do emissor
                    ValidateIssuer = true,
                    //Validar o dominio de que está passando
                    ValidateAudience =  true,
                    //Informacao do dominio
                    ValidAudience = appSettings.ValidoEm,
                    //Informacao do Emissor
                    ValidIssuer = appSettings.Emissor
                };
            });
            #endregion


            return services;
        }
    }
}
