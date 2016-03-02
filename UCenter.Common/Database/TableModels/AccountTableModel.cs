using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;
using UCenter.Common.Models;

namespace UCenter.Common.Database.TableModels
{
    [Export]
    public class AccountTableModel : DatabaseTableModel<AccountEntity>
    {
        [ImportingConstructor]
        public AccountTableModel(IDatabaseClient client, IDatabaseRequestFactory requestFactory)
            : base(client, requestFactory)
        {
        }
    }
}
