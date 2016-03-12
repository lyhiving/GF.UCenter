using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Entities
{
    public class AccountEntity : BaseEntity<AccountEntity>
    {
        public string AccountName { get; set; }

        public string Password { get; set; }

        public string SuperPassword { get; set; }

        public string Token { get; set; }

        public DateTime LastLoginDateTime { get; set; }

        public string Name { get; set; }

        public Sex Sex { get; set; }

        public string IdentityNum { get; set; }

        public string PhoneNum { get; set; }

        public TResponse ToResponse<TResponse>() where TResponse : AccountRequestResponse
        {
            var response = Activator.CreateInstance<TResponse>();
            response.ApplyEntity(this);

            return response;
        }
    }
}
