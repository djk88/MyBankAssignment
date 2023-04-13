using System.ComponentModel.DataAnnotations;

namespace MyBankAssignment.Models
{
    public class BankAccountType
    {
        [Key]
        public string AccountType { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}
