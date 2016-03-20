using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;
using pingpp;
using System.ComponentModel.Composition;
using System.Web.Http;
using System.Threading.Tasks;
using UCenter.Common;

using UCenter.Common.Portable;
using UCenter.CouchBase.Database;

namespace UCenter.Web.ApiControllers
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
        [HttpGet]
        [Route("test")]
        public IHttpActionResult Test()
        {
            logger.Info("in test");
            return CreateSuccessResult("");
        }

        //---------------------------------------------------------------------
        [Route("charge")]
        public IHttpActionResult Charge([FromBody]ChargeInfo info)
        {
            try
            {
                logger.Info("Charge");
                Pingpp.SetApiKey("sk_test_n5S804PuLGOSTuD4KOTGiDC8");
                const string appId = "app_H4yDu5COi1O4SWvz";

                var amount = info.Amount;
                var channel = info.Channel;
                var orderNo = info.OrderNo;

                logger.Info($"amount={amount}");
                logger.Info($"channel={channel}");
                logger.Info($"orderNo={orderNo}");

                var extra = new Dictionary<string, object>();
                if (channel.ToString().Equals("alipay_wap"))
                {
                    extra.Add("success_url", "http://www.yourdomain.com/success");
                    extra.Add("cancel_url", "http://www.yourdomain.com/cancel");
                }
                else if (channel.ToString().Equals("wx_pub"))
                {
                    extra.Add("open_id", "asdfasdfsadfasdf");
                }
                else if (channel.ToString().Equals("upacp_wap"))
                {
                    extra.Add("result_url", "http://www.yourdomain.com/result");
                }
                else if (channel.ToString().Equals("upmp_wap"))
                {
                    extra.Add("result_url", "http://www.yourdomain.com/result?code=");
                }
                else if (channel.ToString().Equals("bfb_wap"))
                {
                    extra.Add("result_url", "http://www.yourdomain.com/result");
                    extra.Add("bfb_login", true);
                }
                else if (channel.ToString().Equals("wx_pub_qr"))
                {
                    extra.Add("product_id", "asdfsadfadsf");
                }
                else if (channel.ToString().Equals("yeepay_wap"))
                {
                    extra.Add("product_category", "1");
                    extra.Add("identity_id", "sadfsdaf");
                    extra.Add("identity_type", 1);
                    extra.Add("terminal_type", 1);
                    extra.Add("terminal_id", "sadfsadf");
                    extra.Add("user_ua", "sadfsdaf");
                    extra.Add("result_url", "http://www.yourdomain.com/result");
                }
                else if (channel.ToString().Equals("jdpay_wap"))
                {
                    extra.Add("success_url", "http://www.yourdomain.com/success");
                    extra.Add("fail_url", "http://www.yourdomain.com/fail");
                    extra.Add("token", "fjdilkkydoqlpiunchdysiqkanczxude");//32 位字符串，京东支付成功后会返回
                }

                var param = new Dictionary<string, object>
                {
                    {"order_no", orderNo},
                    {"amount", amount},
                    {"channel", channel},
                    {"currency", "cny"},
                    {"subject", "test"},
                    {"body", "tests"},
                    {"client_ip", "127.0.0.1"},
                    {"app", new Dictionary<string, string> { { "id", appId } }},
                    {"extra", extra}
                };


                var charge = pingpp.Models.Charge.create(param);
                return CreateSuccessResult(charge);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return CreateErrorResult(UCenterErrorCode.Failed, ex.Message);
            }
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("webhook")]
        public IHttpActionResult WebHook()
        {
            logger.Info("WebHook called, ready to receive events");

            //获取 post 的 event对象                                
            string inputData = Request.Content.ReadAsStringAsync().Result;

            logger.Info("接收到消息\n" + inputData);

            //获取 header 中的签名
            IEnumerable<string> headerValues;
            string sig = string.Empty;
            if (Request.Headers.TryGetValues("x-pingplusplus-signature", out headerValues))
            {
                sig = headerValues.FirstOrDefault();
            }

            //公钥路径（请检查你的公钥 .pem 文件存放路径）
            string path = @"C:\openssl\bin\rsa_public_key.pem";

            //验证签名
            string result = VerifySignedHash(inputData, sig, path);

            var jObject = JObject.Parse(inputData);
            var type = jObject.SelectToken("type");
            logger.Info($"type={type}");
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
        public static string VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_publicKeyFilePath)
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
