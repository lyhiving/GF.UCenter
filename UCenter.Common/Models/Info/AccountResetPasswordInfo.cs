using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    public class AccountResetPasswordInfo
    {
        [StringLength(16, MinimumLength = 6)]
        public string AccountName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(UCenterConst.PasswordRule)]
        public string SuperPassword { get; set; }
    }
}
