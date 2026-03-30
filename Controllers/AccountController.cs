using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;
using System.Security.Claims;

namespace OficinaWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginDAO _loginDAO;

        public AccountController(IConfiguration config)
        {
            _loginDAO = new LoginDAO(config);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = _loginDAO.Autenticar(model.Username, model.Senha);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário ou senha incorretos.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Value.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Value.Nome),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Cadastro() => View();

        [HttpPost]
        public IActionResult Cadastro(CadastroLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _loginDAO.Inserir(model);
            TempData["Sucesso"] = "Usuário cadastrado com sucesso!";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
