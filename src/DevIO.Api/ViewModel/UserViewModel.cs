using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevIO.Api.ViewModel
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        [EmailAddress(ErrorMessage = "O Campo {0} está no formato Inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O Campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        [EmailAddress(ErrorMessage = "O Campo {0} está no formato Inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        public string Password { get; set; }
    }

    public class UserTokenViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimViewModel> Claims { get; set; }
    }

    public class LoginResponseViewModel
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenViewModel User { get; set; }
    }

    public class ClaimViewModel
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
