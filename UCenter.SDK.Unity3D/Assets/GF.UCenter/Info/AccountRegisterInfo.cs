using System;
using System.Collections.Generic;

    [Serializable]
    public class AccountRegisterInfo 
    {
        public string AccountName { get; set; }
    
        public string Password { get; set; }

        public string SuperPassword { get; set; }
    
        public string Name { get; set; }
    
        public string PhoneNum { get; set; }

        public string IdentityNum { get; set; }
    
        public Sex Sex { get; set; }
    }
