using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCenter.Common.Database;
using UCenter.Common.Database.Entities;
using UCenter.Common.Database.TableModels;
using UCenter.Common.Exceptions;
using UCenter.Common.Models;

namespace UCenter.Common.Handler
{
    [Export]
    public class AccountHandler 
    {
        private readonly IDatabaseClient client;
        private readonly IDatabaseRequestFactory requestFactory;
        private readonly AccountTableModel tableModel;

        [ImportingConstructor]
        public AccountHandler(
            IDatabaseClient client,
            IDatabaseRequestFactory requestFactory,
            AccountTableModel tableModel)
        {
            this.client = client;
            this.requestFactory = requestFactory;
            this.tableModel = tableModel;
        }

        public async Task<AccountEntity> RegisterAsync(AccountEntity entity, CancellationToken token)
        {
            var remoteEntities = await this.tableModel.RetrieveEntitiesAsync(e => e.Name == entity.Name || e.PhoneNum == entity.PhoneNum, token);
            if (remoteEntities.Count() > 0)
            {
                throw new AccountExistsException(entity);
            }

            // encrypted the user password.
            entity.Password = EncryptHashManager.ComputeHash(entity.Password);

            return await this.tableModel.InsertEntityAsync(entity, token);
        }


        [Obsolete("Please use RegisterAsync(AccountEntity entity, CancellationToken token)")]
        public async Task<AccountRegisterResponse> RegisterAsync(AccountRegisterRequest requestData, CancellationToken token)
        {
            AccountEntity entity = new AccountEntity()
            {
                AccountName = requestData.acc,
                Password = requestData.pwd,
                Name = requestData.name,
                IdentityNum = requestData.identity_num,
                PhoneNum = requestData.phone_num,
                Sex = Sex.Male,/// requestData.sex_type,
                LastLoginDateTime = DateTime.UtcNow
            };

            AccountRegisterResponse response = new AccountRegisterResponse();

            // todo: what is this acc_name used for?
            //  result.acc_name = request.acc_name;
            try
            {
                await this.tableModel.InsertEntityAsync(entity, token);

                // todo: use reterive entity function instead of the following request.
                var getRequest = this.requestFactory.CreateGetAccountRequest(requestData.acc);
                response = await this.client.ExecuteSingleAsync<AccountRegisterResponse>(getRequest, token);
                response.result = UCenterResult.Success;
            }
            catch (DatabaseException ex)
            {
                if (ex.Number == 1062)
                {
                    response.result = UCenterResult.RegisterAccountExist;
                }

                response.result = UCenterResult.Failed;
            }
            catch (Exception)
            {
                response.result = UCenterResult.Failed;
            }

            return response;
        }

        public IEnumerable<AccountEntity> GetTestData()
        {
            var accounts = new AccountEntity[]
            {
                new AccountEntity { AccountId = 1, AccountName = "Tomato Soup"  },
                new AccountEntity { AccountId = 2, AccountName = "Yo-yo"  },
                new AccountEntity { AccountId = 3, AccountName = "Hammer"}
            };

            return accounts;
        }
    }
}
