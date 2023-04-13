using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankAssignment.Data;
using MyBankAssignment.Models;
using MyBankAssignment.ViewModels;
using System.Collections.Generic;

namespace MyBankAssignment.Repositories
{
    public class ClientAccountRepo
    {
        ApplicationDbContext _db;

        public ClientAccountRepo(ApplicationDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// gets the list of client account ids
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>a list of account ids</returns>
        public List<int> GetClientAccountIds(int clientId) {

            List<int> accountNums =  _db.ClientAccounts.Where(ca => ca.ClientID == clientId)
                                                       .Select(c => c.AccountNum).ToList();
            return accountNums;   
        }

        /// <summary>
        /// gets all client accounts from a single user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>list of a single client account recrds</returns>
        public List<ClientAccountVM> GetAllClientAccounts(string email) {

            BankAccountRepo baRepo = new BankAccountRepo(_db);
            ClientRepo clientRepo = new ClientRepo(_db);
            Client client = clientRepo.GetClient(email);
            List<int> accountNums = GetClientAccountIds(client.ClientID);
            List<BankAccount> bankAccounts = baRepo.GetAllRecords(accountNums);
            List<ClientAccountVM> vm = new List<ClientAccountVM>();

            foreach (var bankAccount in bankAccounts)
            {
                vm.Add(new ClientAccountVM
                {
                    ClientID = client.ClientID,
                    AccountNum = bankAccount.AccountNum,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    AccountType = bankAccount.AccountType,
                    Balance = bankAccount.Balance
                });
            }
            return vm;
        }

        /// <summary>
        /// gets a single client bank account number
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accountNum"></param>
        /// <returns>single account number</returns>
        public ClientAccountVM GetClientAccount(string email, int accountNum)
        {
            BankAccountRepo bankaccountRepo = new BankAccountRepo(_db);
            ClientRepo clientRepo = new ClientRepo(_db);

            Client client = clientRepo.GetClient(email);
            BankAccount bankAccount = bankaccountRepo.GetRecord(accountNum);

            return new ClientAccountVM
            {
                ClientID = client.ClientID,
                AccountNum = bankAccount.AccountNum,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                AccountType = bankAccount.AccountType,
                Balance = bankAccount.Balance
            };
        }

        /// <summary>
        /// gets anentire client account record
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="accountNum"></param>
        /// <returns>client account record</returns>
        public ClientAccount GetClientAccountRecord(int clientId, int accountNum)
        {
            ClientAccount clientAccount = _db.ClientAccounts.Where(ca => ca.ClientID == clientId && ca.AccountNum == accountNum).FirstOrDefault();

            return clientAccount;
        }

        /// <summary>
        /// creates a client account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string CreateClient(ClientAccount account)
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
        /// deletes a clients single account
        /// </summary>
        /// <param name="accountNum"></param>
        /// <param name="email"></param>
        /// <returns>none</returns>
        public string DeleteClientAccount(int accountNum, string email)
        {
            ClientRepo cRepo = new ClientRepo(_db);
            Client client = cRepo.GetClient(email);

            var clientAccountToDelete = GetClientAccountRecord(client.ClientID, accountNum);

            _db.ClientAccounts.Remove(clientAccountToDelete);

            return "";
        }
    }
}
