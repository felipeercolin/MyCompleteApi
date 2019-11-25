using System;
using System.Threading.Tasks;
using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private readonly ILogger _logger;
        public TesteController(INotificador notificador, IUser appUser, ILogger<TesteController> logger) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Valor()
        {
            try
            {
                var i = 0;
                var result = 42 / i;
            }
            catch (DivideByZeroException erro)
            {
                erro.Ship(HttpContext);
                await erro.ShipAsync(HttpContext);//Method Extention do Elmah para enviar exececoes para o logging
            }


            //Menos Impactante, usado pra DEV, ex: comecou 5h e terminou 7h.
            _logger.LogTrace("log de trace");
            //usando no DEV
            _logger.LogDebug("log de debug");
            // INFO
            _logger.LogInformation("log de informacao");
            //ERRO 404
            _logger.LogWarning("log de aviso");
            //QUANDO HOUVER ERRO
            _logger.LogError("log de erro");
            //PERFOMACE
            _logger.LogCritical("log de Problema Critico");

            return "Sou a V2";
        }
    }
}
