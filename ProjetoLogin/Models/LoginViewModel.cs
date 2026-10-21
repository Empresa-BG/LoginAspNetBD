using System.ComponentModel.DataAnnotations;

namespace ProjetoLogin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Informe o email")]
        [EmailAddress(ErrorMessage ="email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;
    }
}
