using MyBankAssignment.Models;
using System.ComponentModel.DataAnnotations;

namespace MyBankAssignment.ViewModels
{
    public class ClientAccountVM
    {
        [Key]
        [Display(Name = "Client ID")]
        public int ClientID { get; set; }

        [Display(Name = "Account Number")]
        public int AccountNum { get; set; }

        [Display(Name ="Last Name")]
        [Required(ErrorMessage = "Last Name Required")]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z-_]*$", ErrorMessage = "Please use aplhabetical charachters only")]
        [MaxLength(50, ErrorMessage = "Last name only allows 50 charachters")]
        public string LastName { get; set; }

        [Display(Name ="First Name")]
        [Required(ErrorMessage = "First Name Required")]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z-_]*$", ErrorMessage = "Please use aplhabetical charachters only")]
        [MaxLength(50, ErrorMessage = "First name only allows 50 charachters")]
        public string FirstName { get; set; }

        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Display(Name = "Balance")]
        [Range(1, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessage = "only two decimal allowed")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Balance { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string? Message { get; set; }
    }
}
