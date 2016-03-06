using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountLoginInfo : ValidatableInfo
    {
        [StringLength(16, MinimumLength = 6)]
        public string AccountName { get; set; }

        [Required]
        public string Password;
    }

}
