using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;

namespace GF.UCenter.Common.Models
{
    [DataContract]
    public class AccountRegisterRequestInfo : AccountRegisterInfo
    {
        // todo: Set the valide rules of following properties.

        [DataMember]
        // [RegularExpression(UCenterModelRules.AccountNameRule)]
        [Required]
        public override string AccountName { get; set; }

        [DataMember]
        // [RegularExpression(UCenterModelRules.PasswordRule)]
        [Required]
        public override string Password { get; set; }

        [DataMember]
        // [RegularExpression(UCenterModelRules.PasswordRule)]
        [Required]
        public override string SuperPassword { get; set; }

        [DataMember]
        // [StringLength(32, MinimumLength = 4)]
        public override string Name { get; set; }

        [DataMember]
        // [EmailAddress]
        public override string Email { get; set; }
    }
}
