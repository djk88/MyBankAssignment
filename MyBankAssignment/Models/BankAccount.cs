using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyBankAssignment.Models
{
    public class BankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountNum { get; set; }
        public string AccountType { get; set; }
        public double Balance{ get; set; }

        public virtual ICollection<ClientAccount>? ClientAccounts { get; set; }

        public virtual BankAccountType BankAccountTypes { get; set; }
    }

}
