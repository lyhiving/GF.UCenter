using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using GF.UCenter.Common;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using Newtonsoft.Json.Linq;
using NLog;
using pingpp;
using Logger = NLog.Logger;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/payment")]
    [TraceExceptionFilter("PaymentApiController")]
    public class PaymentApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private Logger logger = LogManager.GetCurrentClassLogger();

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public PaymentApiController(CouchBaseContext db)
            : base(db)
        {
        }

        //---------------------------------------------------------------------
        [Route("charge")]
        public IHttpActionResult Charge([FromBody] ChargeInfo info)
        {
            logger.Info($"AppServer请求读取Data\nAppId={info.AppId}\nAccountId={info.AccountId}");

            try
            {
                Pingpp.SetApiKey("sk_test_zXnD8KKOyfn1vDuj9SG8ibfT");

                string appId = "app_H4yDu5COi1O4SWvz";
                var r = new Random();
                string orderNoPostfix = r.Next(0, 1000000).ToString("D6");
                string orderNo = $"{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{orderNoPostfix}";
                double amount = info.Amount;
                string channel = "alipay";
                string currency = "cny";

                var param = new Dictionary<string, object>
                {
                    {"livemode", false},
                    {"order_no", orderNo},
                    {"amount", amount},
                    {"channel", channel},
                    {"currency", currency},
                    {"subject", info.Subject},
                    {"body", info.Body},
                    {"description", info.Description},
                    {"client_ip", info.ClientIp},
                    {"app", new Dictionary<string, string> {{"id", appId}}}
                };

                var charge = pingpp.Models.Charge.create(param);

                return CreateSuccessResult(charge);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "创建Charge失败");
                return CreateErrorResult(UCenterErrorCode.PaymentCreateChargeFailed, ex.Message);
            }
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("webhook")]
        public IHttpActionResult WebHook()
        {
            logger.Info("UCenter接收到ping++回调消息");

            //获取 post 的 event对象
            string inputData = Request.Content.ReadAsStringAsync().Result;
            logger.Info("消息内容\n" + inputData);

            //获取 header 中的签名
            IEnumerable<string> headerValues;
            string sig = string.Empty;
            if (Request.Headers.TryGetValues("x-pingplusplus-signature", out headerValues))
            {
                sig = headerValues.FirstOrDefault();
            }

            //公钥路径（请检查你的公钥 .pem 文件存放路径）
            string path = @"~/App_Data/rsa_public_key.pem";

            //验证签名
            string result = VerifySignedHash(inputData, sig, path);

            var jObject = JObject.Parse(inputData);
            var type = jObject.SelectToken("type");

            if (type.ToString() == "charge.succeeded" || type.ToString() == "refund.succeeded")
            {
                // TODO what you need do
                //Response.StatusCode = 200;
            }
            else
            {
                // TODO what you need do
                //Response.StatusCode = 500;
            }

            return CreateSuccessResult("Success received order info");
        }

        //---------------------------------------------------------------------
        public static string VerifySignedHash(string str_DataToVerify, string str_SignedData,
            string str_publicKeyFilePath)
        {
            byte[] SignedData = Convert.FromBase64String(str_SignedData);

            UTF8Encoding ByteConverter = new UTF8Encoding();
            byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
            try
            {
                string sPublicKeyPEM = System.IO.File.ReadAllText(str_publicKeyFilePath);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.PersistKeyInCsp = false;
                rsa.LoadPublicKeyPEM(sPublicKeyPEM);

                if (rsa.VerifyData(DataToVerify, "SHA256", SignedData))
                {
                    return "verify success";
                }
                else
                {
                    return "verify fail";
                }

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return "verify error";
            }

        }

    }
}
