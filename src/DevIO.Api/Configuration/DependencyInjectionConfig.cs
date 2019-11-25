
using DevIO.Api.Extentions;
using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DevIO.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependences(this IServiceCollection service)
        {
            service.AddScoped<MeuDbContext>();
            service.AddScoped<IProdutoRepository, ProdutoRepository>();
            service.AddScoped<IFornecedorRepository, FornecedorRepository>();
            service.AddScoped<IEnderecoRepository, EnderecoRepository>();

            service.AddScoped<INotificador, Notificador>();
            service.AddScoped<IFornecedorService, FornecedorService>();
            service.AddScoped<IProdutoService, ProdutoService>();

            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            service.AddScoped<IUser, AspNetUser>();

            service.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return service;
        }
    }
}

