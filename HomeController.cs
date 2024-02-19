/*using HalloDoc.DBModels;*/
using HalloDoc.Models;
using HalloDoc.Models.ViewModels;
using HalloDocService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using HalloDocService.ViewModels;
using HalloDocRepo.DBModels;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HalloDocContext _context;
        private readonly ILoginService _loginService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(HalloDocContext context, ILoginService loginService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _loginService = loginService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> login([Bind("Email", "PasswordHash")] LoginUser1 loginUser)
        {
            

           

            if (ModelState.IsValid)
            {
                int Id = _loginService.Login(loginUser);
                _httpContextAccessor.HttpContext.Session.SetInt32("ID", Id);

                if (Id == 0)
                {
                    return RedirectToAction("login", "Home");
                }
               
                    return RedirectToAction(nameof(Dashboard), "Patient"); 
                
            }
            
                return View("~/Views/Home/login.cshtml"); 
            


        }

        public IActionResult SubmitRequest()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}