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
        /// it must be start with letter or _, and follow by letter, _ or number, end with latter or number.
        /// and the length between 6 and 16
        /// </summary>
        [Required]
        [RegularExpression(@"^[a-zA-Z_]\w{4,14}[a-zA-Z0-9]$")]
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// The passsword should be follow the following rules:
        ///     At least one upper case letter
        ///     At least one lower case letter
        ///     At least one number, that is 0-9
        ///     At least one special symbol, that is: !@#$%*()_+^&}{:;?.
        /// </summary>
        [Required]
        [RegularExpression(@"^(?=^.{6,25}$)(?=(?:.*?\d){1})(?=.*[a-z])(?=(?:.*?[A-Z]){1})(?=(?:.*?[!@#$%*()_+^&}{:;?.]){1})(?!.*\s)[0-9a-zA-Z!@#$%*()_+^&]*$")]
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
