using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountRegisterInfo : ValidatableInfo
    {

        /// <summary>
        /// Gets or sets the account name
        /// </summary>
        [Required]
        [RegularExpression(UCenterConst.AccountNameRule)]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [RegularExpression(UCenterConst.PasswordRule)]
        public string Password { get; set; }

        public string SuperPassword { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNum { get; set; }

        public string IdentityNum { get; set; }

        [Required]
        public Sex Sex { get; set; }
    }
}
