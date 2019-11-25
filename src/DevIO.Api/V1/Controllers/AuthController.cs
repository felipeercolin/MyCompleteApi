using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DevIO.Api.Controllers;
using DevIO.Api.Extentions;
using DevIO.Api.ViewModel;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevIO.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public AuthController(INotificador notificador,
                              SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              IUser user, ILogger<AuthController> logger) : base(notificador, user)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _appSettings = appSettings.Value;
        }


        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel vmodel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = vmodel.Email,
                Email = vmodel.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, vmodel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomResponse(await GerarJwt(user.Email));
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(vmodel);
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel vmodel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(vmodel.Email, vmodel.Password, false, true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Usuario {vmodel.Email} logado com Sucesso");
                return CustomResponse(await GerarJwt(vmodel.Email));
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário Temporariamente Bloqueado por Tentativas Inválidas");
                return CustomResponse(vmodel);
            }
            
            NotificarErro("Usuário ou Senha Inválidos.");
            return CustomResponse(vmodel);
        }

        //private string GerarJwt()//Token simples
        //private async Task<string> GerarJwt(string email)//token com as claims
        private async Task<LoginResponseViewModel> GerarJwt(string email)//Passando Mais informacoes para o client
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixExpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixExpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }


            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);


            #region Forma Basica - Sem permissoes, email, etcs
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExiperacaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            }); 
            #endregion

            var encondedToken = tokenHandler.WriteToken(token);

            //return encondedToken;

            var response = new LoginResponseViewModel
            {
                AccessToken = encondedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExiperacaoHoras).TotalSeconds,
                User = new UserTokenViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(cd => new ClaimViewModel{Type = cd.Type, Value = cd.Value})
                }
            };

            return response;
        }

        private static long ToUnixExpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}