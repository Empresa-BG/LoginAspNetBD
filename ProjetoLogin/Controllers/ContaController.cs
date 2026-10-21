using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoLogin.Models;
using ProjetoLogin.Repositories;
using System.Security.Claims;

namespace ProjetoLogin.Controllers
{
    public class ContaController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        //CONSTRUTOR
        public ContaController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }
        [HttpGet]
        public IActionResult CriarConta() => View();

        [HttpPost]
        public async Task<IActionResult> CriarConta(string nome,string email, string senha)
        {
            if(string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha todos os campos";
                return View();
            }
            var usuarioExistente = await _usuarioRepositorio.BuscarEmail(email);
            if (usuarioExistente != null)
            {
                ViewBag.Erro = "Email já está cadastrado";
                return View();
            }

            var novoUsuario = new UsuarioModel { Nome = nome, Email = email };
            var sucesso = await _usuarioRepositorio.Cadastrar(novoUsuario, senha);

            if (sucesso)
            {
                TempData["Sucesso"]="Cadastro Realizado com sucesso! faça seu Login";
                return RedirectToAction(nameof(Login));
            }
            ViewBag.Erro = "Erro ao cadastrar usuário";
            return View();

        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View();
            var usuario = await _usuarioRepositorio.BuscarEmail(model.Email);
            if(usuario == null)
            {
                ModelState.AddModelError(string.Empty, "email ou senha inválidas");
                return View(model);
            }

            var passwordHash = new PasswordHasher<UsuarioModel>();
            var resultado = passwordHash.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha);

            if(resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            var claimsIdentify = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentify));

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
