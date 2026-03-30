using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaWeb.DAL;
using OficinaWeb.Models;

namespace OficinaWeb.Controllers
{
    [Authorize]
    public class OficinaController : Controller
    {
        private readonly OficinaDAO _dao;
        private readonly LoginDAO _loginDAO;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _extsPermitidas = { ".png", ".jpg", ".jpeg", ".gif", ".webp" };

        public OficinaController(IConfiguration config, IWebHostEnvironment env)
        {
            _dao = new OficinaDAO(config);
            _loginDAO = new LoginDAO(config);
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var oficina = _dao.Carregar() ?? new OficinaViewModel();
            oficina.LogoUrl = EncontrarLogoUrl();
            ViewBag.Usuarios = _loginDAO.ListarTodos();
            return View(oficina);
        }

        [HttpPost]
        public async Task<IActionResult> Index(OficinaViewModel model, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
            {
                model.LogoUrl = EncontrarLogoUrl();
                return View(model);
            }

            if (_dao.Existe())
                _dao.Atualizar(model);
            else
                _dao.Inserir(model);

            if (logoFile != null && logoFile.Length > 0)
            {
                var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                if (!_extsPermitidas.Contains(ext))
                {
                    ModelState.AddModelError("", "Formato de imagem não suportado. Use PNG, JPG ou GIF.");
                    model.LogoUrl = EncontrarLogoUrl();
                    return View(model);
                }

                if (logoFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "A imagem não pode ter mais de 2 MB.");
                    model.LogoUrl = EncontrarLogoUrl();
                    return View(model);
                }

                var imgDir = Path.Combine(_env.WebRootPath, "img");
                Directory.CreateDirectory(imgDir);

                // Remove logo anterior (qualquer extensão)
                foreach (var e in _extsPermitidas)
                {
                    var old = Path.Combine(imgDir, "logo_oficina" + e);
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }

                var destino = Path.Combine(imgDir, "logo_oficina" + ext);
                using var stream = new FileStream(destino, FileMode.Create);
                await logoFile.CopyToAsync(stream);
            }

            TempData["Sucesso"] = "Dados da oficina salvos com sucesso!";
            return RedirectToAction("Index");
        }

        // ── Gestão de Usuários ────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarUsuario(CadastroLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var oficina = _dao.Carregar() ?? new OficinaViewModel();
                oficina.LogoUrl = EncontrarLogoUrl();
                ViewBag.Usuarios = _loginDAO.ListarTodos();
                ViewBag.ErroCriarUsuario = true;
                return View("Index", oficina);
            }
            _loginDAO.Inserir(model);
            TempData["Sucesso"] = $"Usuário '{model.Username}' criado com sucesso!";
            return RedirectToAction("Index", null, "secUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarUsuario(AlterarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Dados inválidos para alteração do usuário.";
                return RedirectToAction("Index", null, "secUsuarios");
            }
            _loginDAO.Atualizar(model);
            TempData["Sucesso"] = "Usuário alterado com sucesso!";
            return RedirectToAction("Index", null, "secUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirUsuario(int id)
        {
            _loginDAO.Excluir(id);
            TempData["Sucesso"] = "Usuário excluído com sucesso!";
            return RedirectToAction("Index", null, "secUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverLogo()
        {
            var imgDir = Path.Combine(_env.WebRootPath, "img");
            foreach (var e in _extsPermitidas)
            {
                var f = Path.Combine(imgDir, "logo_oficina" + e);
                if (System.IO.File.Exists(f)) System.IO.File.Delete(f);
            }
            TempData["Sucesso"] = "Logo removido com sucesso!";
            return RedirectToAction("Index");
        }

        private string EncontrarLogoUrl()
        {
            var imgDir = Path.Combine(_env.WebRootPath, "img");
            if (!Directory.Exists(imgDir)) return "";
            foreach (var e in _extsPermitidas)
            {
                if (System.IO.File.Exists(Path.Combine(imgDir, "logo_oficina" + e)))
                    return "/img/logo_oficina" + e;
            }
            return "";
        }
    }
}
