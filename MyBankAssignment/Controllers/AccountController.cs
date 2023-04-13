using Microsoft.AspNetCore.Mvc;
using MyBankAssignment.Data;
using MyBankAssignment.ViewModels;
using MyBankAssignment.Models;
using System.Net.WebSockets;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Update;
using MyBankAssignment.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBankAssignment.Utilities;

namespace MyBankAssignment.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext _db;

        public AccountController(ApplicationDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// gets all bank accounts owned by the logged in user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <returns>shows a list of all bank accounts owned by one user </returns>
        public IActionResult Index(string message, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ClientRepo clientRepo = new ClientRepo(_db);
            Client clientID = clientRepo.GetClient(User.Identity.Name);
            HttpContext.Session.SetString("FirstName", clientID.FirstName);
            HttpContext.Session.SetString("ClientID", clientID.ClientID.ToString());

            ViewData["Message"] = message;

            if (message == null)
            {
                message = "";
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ClientAccountRepo clientAccountRepo = new ClientAccountRepo(_db);

            IQueryable<ClientAccountVM> vm = clientAccountRepo.GetAllClientAccounts(User.Identity.Name).AsQueryable();


            if (!String.IsNullOrEmpty(searchString))
            {
                vm = vm.Where(x => x.AccountType.Contains(searchString));
            }

            if (string.IsNullOrEmpty(sortOrder))
            {
                ViewData["accountSortParm"] = "AccountNum";
            }
            else
            {
                ViewData["accountSortParm"] = sortOrder == "AccountNum" ?
                                                        "name_desc" : "AccountNum";
            }

            ViewData["AccountTypeSort"] = sortOrder == "AccountType" ?
                                                     "AccountDesc" : "AccountType";


            switch (sortOrder)
            {
                case "AccountNum":
                    vm = vm.OrderByDescending(p => p.AccountNum);
                    break;
                case "AccountType":
                    vm = vm.OrderBy(p => p.AccountType);
                    break;
                case "AccountDesc":
                    vm = vm.OrderByDescending(p => p.AccountType);
                    break;
                default:
                    vm = vm.OrderBy(p => p.AccountNum);
                    break;
            }

            int pageSize = 2;
            return View(PaginatedList<ClientAccountVM>.Create(vm.AsNoTracking(), page ?? 1, pageSize));

        }

        /// <summary>
        /// gets users client information and bank information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns>shows a list of details from a single users bank account</returns>
        public IActionResult Details(int id, string msg)
        {
            ClientAccountRepo clientAccountRepo = new ClientAccountRepo(_db);
            ClientAccountVM clientAccountVM = clientAccountRepo.GetClientAccount(User.Identity.Name, id);

            clientAccountVM.Message = msg;

            return View(clientAccountVM);
        }

        /// <summary>
        /// allows user to create a bank account and enter in their account type and balance 
        /// </summary>
        /// <returns>to a detail view of the new account the user created</returns>
        public IActionResult Create()
        {
            ViewData["AccountType"] = new SelectList(_db.BankAccountTypes, "AccountType", "AccountType");
            return View();
        }
        [HttpPost]
        public IActionResult Create([Bind("AccountNum, AccountType, Balance")] BankAccountVM account)
        {
            var registeredUser = _db.Clients.Where(ca => ca.Email == User.Identity.Name).FirstOrDefault();
            var clientID = registeredUser.ClientID;

            BankAccountRepo baRepo = new BankAccountRepo(_db);
            ClientAccountRepo caRepo = new ClientAccountRepo(_db);

            string message = "";

            if (ModelState.IsValid)
            {
                try
                {
                    BankAccount bankAccount = new BankAccount()
                    {
                        AccountType = account.AccountType,
                        Balance = account.Balance
                    };
                    baRepo.CreateRecord(bankAccount);

                    ClientAccount newClientAccount = new ClientAccount()
                    {
                        AccountNum = bankAccount.AccountNum,
                        ClientID = clientID
                    };
                    message = caRepo.CreateClient(newClientAccount);

                    message = $"Success creating your {account.AccountType}, your new account number is {newClientAccount.AccountNum}";

                    return RedirectToAction("Details", "Account", new { id = newClientAccount.AccountNum, msg = message });
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return RedirectToAction("Index");
        }

        
        /// <summary>
        /// allows users to edit some of their client info and bank account info
        /// </summary>
        /// <param name="id"></param>
        /// <returns>shows a message upon a successful edit </returns>
        public IActionResult Edit(int id)
        {
            ClientAccountRepo clientAccountRepo = new ClientAccountRepo(_db);
            ClientAccountVM clientAccountVM = clientAccountRepo.GetClientAccount(User.Identity.Name, id);

            ViewData["AccountType"] = new SelectList(_db.BankAccountTypes, "AccountType", "AccountType");

            return View(clientAccountVM);
        }
        [HttpPost]
        public IActionResult Edit([Bind("ClientID, AccountNum, AccountType, FirstName, LastName, Balance, Email")] ClientAccountVM account)
        {
            BankAccountRepo bankAccountRepo = new BankAccountRepo(_db);
            ClientRepo clientRepo = new ClientRepo(_db);
            string message = "";

            if (ModelState.IsValid)
            {
                try
                {                  
                    Client client = new Client()
                    {
                        ClientID = account.ClientID,
                        FirstName = account.FirstName,
                        LastName = account.LastName,
                        Email = account.Email
                    };
                    clientRepo.EditClient(client);

                    BankAccount bankAccount = new BankAccount()
                    {
                        Balance = account.Balance,
                        AccountNum = account.AccountNum,
                        AccountType = account.AccountType
                    };
                    message = bankAccountRepo.EditRecord(bankAccount);

                    return RedirectToAction("Details", "Account", new {id = bankAccount.AccountNum, msg = message});
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            ViewData["AccountType"] = new SelectList(_db.BankAccountTypes, "AccountType", "AccountType");
            return View(account);
        }

        /// <summary>
        /// allows users to delete a single bank accounr record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a messge when the delete is successfull</returns>
        public IActionResult Delete(int id)
        {
            BankAccountRepo bankAccountRepo = new BankAccountRepo(_db);

            var bankRecord = bankAccountRepo.GetRecord(id);

            BankAccountVM bankAccountVM = new BankAccountVM()
            {
                AccountType = bankRecord.AccountType,
                AccountNum = bankRecord.AccountNum,
                Balance = bankRecord.Balance
            };
            return View(bankAccountVM);
        }

        /// <summary>
        /// Post Delete
        /// </summary>
        /// <param name="AccountNum"></param>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int AccountNum, int ClientID)
        {
            ViewData["Message"] = "";

            try
            {
                BankAccountRepo bankAccountRepo = new BankAccountRepo(_db);
                var deleted = bankAccountRepo.DeleteRecord(AccountNum, User.Identity.Name);
                ViewData["Message"] = "Successfully Deleted";
            }
            catch (Exception e)
            {
                ViewData["Message"] = e.Message;
            }
            return RedirectToAction("Index", ViewData);
        }
    }
}

