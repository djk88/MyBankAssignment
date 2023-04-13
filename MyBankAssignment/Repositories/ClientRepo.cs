using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using MyBankAssignment.Data;
using MyBankAssignment.Models;
using MyBankAssignment.ViewModels;
using static System.Formats.Asn1.AsnWriter;

namespace MyBankAssignment.Repositories
{
    public class ClientRepo
    {
        ApplicationDbContext _db;

        public ClientRepo(ApplicationDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Gets a clients id by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>client id</returns>
        public Client GetClient(string email)
        {
            Client client = _db.Clients.Where(c => c.Email == email).FirstOrDefault();
        
            return client;
        }

        /// <summary>
        /// creates client upon registering
        /// </summary>
        /// <param name="client"></param>
        /// <returns>none</returns>
        public string CreateClient(Client client)
        {
            string messege = "";
            _db.Add(client);
            _db.SaveChanges();
            return messege = "success";
        }

        /// <summary>
        /// edit client info
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string EditClient(Client account)
        {
            string ViewMessage = "";

            try
            {
                _db.Update(account);
                _db.SaveChanges();
                ViewMessage = $"{account} has been added.";
            }
            catch (Exception ex)
            {
                ViewMessage = ex.Message;
            }
            return ViewMessage;
        }
    }
}
