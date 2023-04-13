using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using MyBankAssignment.Models;
using MyBankAssignment.Data;
using MyBankAssignment.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MyBankAssignment.Repositories
{
    public class BankAccountRepo
    {
        ApplicationDbContext _db;

        public BankAccountRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// this method gets all account records
        /// </summary>
        /// <param name="bankNums"></param>
        /// <returns>a list of all bank account numbers</returns>
        public List<BankAccount> GetAllRecords(List<int> bankNums)
        {
            List<BankAccount> bankAccounts = new List<BankAccount>();

            if (bankNums != null && bankNums.Count > 0)
            {
                bankAccounts = _db.BankAccounts.Where(ba => bankNums.Contains(ba.AccountNum))
                                               .Select(b => new BankAccount { AccountNum = b.AccountNum,
                                                                              AccountType = b.AccountType,  
                                                                              Balance = b.Balance }).ToList();
            }
            return bankAccounts;
        }

        /// <summary>
        /// gets the entire bank account row from a single user
        /// </summary>
        /// <param name="accountNum"></param>
        /// <returns>returns bank account row</returns>
        public BankAccount GetRecord(int accountNum)
        {
            BankAccount bankAccount = new BankAccount();

            bankAccount = _db.BankAccounts.Where(ca => ca.AccountNum == accountNum).FirstOrDefault() ;

            return bankAccount;
        }


        /// <summary>
        /// creates a single bank account number
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string CreateRecord(BankAccount account)
        {
            string ViewMessage = "";

            try
            {
                _db.Add(account);
                _db.SaveChanges();
                ViewMessage = $"{account} has been added.";
            }
            catch (Exception ex)
            {
                ViewMessage = ex.Message;
            }
            return ViewMessage;
        }

        /// <summary>
        /// editing bank account info
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string EditRecord(BankAccount account)
        {
            string ViewMessage = "";

            try
            {
                _db.Update(account);
                _db.SaveChanges();
                ViewMessage = $"Success editing {account.AccountType} account No.{account.AccountNum}";
            }
            catch (Exception ex)
            {
                ViewMessage = ex.Message;
            }
            return ViewMessage;
        }



        /// <summary>
        /// deletes a client single account by account number
        /// </summary>
        /// <param name="accountNum"></param>
        /// <param name="email"></param>
        /// <returns>none</returns>
        public string DeleteRecord(int accountNum, string email)
        {
            ClientAccountRepo clientAccountRepo = new ClientAccountRepo(_db);

            clientAccountRepo.DeleteClientAccount(accountNum, email);

            _db.BankAccounts.Remove(GetRecord(accountNum));
            _db.SaveChanges();

            var message = "Bank account deleted";

            return message;
        }
    }
}

