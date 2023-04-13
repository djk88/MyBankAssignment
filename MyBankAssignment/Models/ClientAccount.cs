using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyBankAssignment.Models
{
    public class ClientAccount
    {
        [Key, Column(Order = 0)]
        public int ClientID { get; set; }

        [Key, Column(Order = 1)]
        public int AccountNum { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Client Client { get; set; }
    }
}
