using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBankAssignment.Data;
using MyBankAssignment.Models;
using MyBankAssignment.Repositories;
using System.Diagnostics;

namespace MyBankAssignment.Controllers7
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var c = User;
            if (User != null && User.Identity.IsAuthenticated)
            {
                ClientRepo clientRepo = new ClientRepo(_context);
                Client clientID = clientRepo.GetClient(User.Identity.Name);
                HttpContext.Session.SetString("FirstName", clientID.FirstName);
                HttpContext.Session.SetString("ClientID", clientID.ClientID.ToString());
            }
            return View();

        }

        public IActionResult Privacy()
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