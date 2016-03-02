using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Exceptions
{
    public class AccountExistsException : Exception
    {
        public AccountEntity Account { get; private set; }

        public AccountExistsException(AccountEntity entity)
            : base($"The Account:{entity.Name} or PhoneNumber: {entity.PhoneNum} already exists.")
        {
            this.Account = entity;
        }
    }
}
