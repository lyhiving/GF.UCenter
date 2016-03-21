using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Portable;
using UCenter.SDK.AppClient;

namespace GF.UCenter.Test.AppClient
{
    class Program
    {
        //---------------------------------------------------------------------
        static void Main(string[] args)
        {
            Task task = new Task(run);
            task.Start();

            task.Wait();

            Console.Read();
        }

        //---------------------------------------------------------------------
        static async void run()
        {
            string host = "http://cragonucenter.chinacloudsites.cn";
            UCenterClient c = new UCenterClient(host);

            AccountRegisterInfo account_register_info = new AccountRegisterInfo();
            account_register_info.AccountName = "test1010";
            account_register_info.Password = "123456";
            account_register_info.SuperPassword = "12345678";
            AccountRegisterResponse r = await c.AccountRegisterAsync(account_register_info);

            Console.WriteLine(r.AccountId);
        }
    }
}
