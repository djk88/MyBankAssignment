using Microsoft.EntityFrameworkCore;
using MyBankAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankAssignment.ViewModels
{
    [Keyless]
    public class BankAccountVM
    {
        [Display(Name = "Account Number")]
        public int? AccountNum { get; set; }

        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Display(Name = "Balance")]
        [Range(1, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        [RegularExpression("^[0-9]*(\\.[0-9]{1,2})?$", ErrorMessage = "Please enter in a valid balance")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Balance { get; set; }
        public string? Message { get; set; }

    }
}
