using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using FluentAssertions;
using Lib.core;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MessageBus;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Admin.Consumer.Consumers
{
    /*
     {
"Message": "OK",
"RequestId": "873043ac-bcda-44db-9052-2e204c6ed20f",
"BizId": "607300000000000000^0",
"Code": "OK"
}
         */

    class AliyunSmsRes
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// 发送实名认证短信
    /// </summary>
    [QueueConfig(ConfigSet.MessageBus.Queue.User)]
    public class IdConfirmSmsConsumer : IMessageConsumer<UserPhoneBindSmsMessage>, Lib.core.IFinder
    {
        private readonly IServiceProvider provider;
        public IdConfirmSmsConsumer(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task Consume(IMessageConsumeContext<UserPhoneBindSmsMessage> context)
        {
            try
            {
                context.Message.Phone.Should().NotBeNullOrEmpty();
                context.Message.Code.Should().NotBeNullOrEmpty();

                var config = provider.ResolveConfig_();
                string get_config(string key)
                {
                    var config_res = config[$"aliyun:sms:{key}"];
                    config_res.Should().NotBeNullOrEmpty();
                    return config_res;
                }

                //发送短信
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", get_config("key"), get_config("secret"));
                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                // request.Protocol = ProtocolType.HTTP;
                request.AddQueryParameters("PhoneNumbers", context.Message.Phone);
                request.AddQueryParameters("SignName", get_config("sign_name"));
                request.AddQueryParameters("TemplateCode", get_config("template_code"));
                request.AddQueryParameters("TemplateParam", new { code = context.Message.Code }.ToJson());

                var response = client.GetCommonResponse(request);
                var sms_res_content = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                sms_res_content.Should().NotBeNullOrEmpty();

                var sms_res = sms_res_content.JsonToEntity<AliyunSmsRes>();
                if (sms_res.Code?.ToLower() != "ok")
                {
                    throw new MsgException(sms_res.Message ?? "sms error");
                }
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                var _logger = provider.Resolve_<ILogger<IdConfirmSmsConsumer>>();
                _logger.AddErrorLog(nameof(IdConfirmSmsConsumer), e);
            }
        }
    }
}
