using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WCloud.Framework.Wechat.Models;

namespace WCloud.Framework.Wechat.Pay
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg) { }
        public WxPayException(string msg, Exception inner) : base(msg, inner) { }

        public object Info { get; set; }
    }

    public abstract class WxData
    {
        public const string SUCCESS = "SUCCESS";

        private readonly IDictionary<string, string> dict;

        public WxData(IDictionary<string, string> dict)
        {
            dict.Should().NotBeNull();
            this.dict = dict;
        }

        public IDictionary<string, string> Parameters => this.dict;

        public string TryGetValue(string key)
        {
            if (this.dict.TryGetValue(key, out var data))
            {
                return data;
            }
            return null;
        }

        public int GetIntOrThrow(string key)
        {
            var data = this.TryGetValue(key);
            if (int.TryParse(data, out var val))
            {
                return val;
            }
            throw new WxPayException($"{data}无法转换成int");
        }
    }

    public class WxPayUnifiedOrderResponse : WxData
    {
        public WxPayUnifiedOrderResponse(IDictionary<string, string> dict) : base(dict) { }

        public string return_code => this.TryGetValue(nameof(return_code));
        public string return_msg => this.TryGetValue(nameof(return_msg));
        public string appid => this.TryGetValue(nameof(appid));
        public string mch_id => this.TryGetValue(nameof(mch_id));
        public string device_info => this.TryGetValue(nameof(device_info));
        public string nonce_str => this.TryGetValue(nameof(nonce_str));
        public string sign => this.TryGetValue(nameof(sign));
        public string result_code => this.TryGetValue(nameof(result_code));
        public string err_code => this.TryGetValue(nameof(err_code));
        public string err_code_des => this.TryGetValue(nameof(err_code_des));
        public string trade_type => this.TryGetValue(nameof(trade_type));
        public string prepay_id => this.TryGetValue(nameof(prepay_id));
        public string code_url => this.TryGetValue(nameof(code_url));

        public bool IsReturnCodeSuccess() => this.return_code == SUCCESS;

        public bool IsSuccess() => this.IsReturnCodeSuccess() && this.result_code == SUCCESS;
    }

    /// <summary>
    /// https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_7&index=8
    /// </summary>
    public class WxPayNotifyData : WxData
    {
        public WxPayNotifyData(IDictionary<string, string> dict) : base(dict) { }

        public string return_code => this.TryGetValue(nameof(return_code));
        public string return_msg => this.TryGetValue(nameof(return_msg));

        public string appid => this.TryGetValue(nameof(appid));
        public string mch_id => this.TryGetValue(nameof(mch_id));
        public string device_info => this.TryGetValue(nameof(device_info));
        public string nonce_str => this.TryGetValue(nameof(nonce_str));
        public string sign => this.TryGetValue(nameof(sign));
        public string sign_type => this.TryGetValue(nameof(sign_type));
        public string result_code => this.TryGetValue(nameof(result_code));
        public string err_code => this.TryGetValue(nameof(err_code));
        public string err_code_des => this.TryGetValue(nameof(err_code_des));
        public string openid => this.TryGetValue(nameof(openid));
        public string is_subscribe => this.TryGetValue(nameof(is_subscribe));
        public string trade_type => this.TryGetValue(nameof(trade_type));
        public string bank_type => this.TryGetValue(nameof(bank_type));
        public string total_fee => this.TryGetValue(nameof(total_fee));
        public string settlement_total_fee => this.TryGetValue(nameof(settlement_total_fee));
        public string fee_type => this.TryGetValue(nameof(fee_type));
        public string cash_fee => this.TryGetValue(nameof(cash_fee));
        public string cash_fee_type => this.TryGetValue(nameof(cash_fee_type));
        public string coupon_fee => this.TryGetValue(nameof(coupon_fee));
        public string coupon_count => this.TryGetValue(nameof(coupon_count));
        public string transaction_id => this.TryGetValue(nameof(transaction_id));
        public string out_trade_no => this.TryGetValue(nameof(out_trade_no));
        public string attach => this.TryGetValue(nameof(attach));
        public string time_end => this.TryGetValue(nameof(time_end));

        public bool IsReturnCodeSuccess() => this.return_code == SUCCESS;

        public bool IsSuccess() => this.IsReturnCodeSuccess() && this.result_code == SUCCESS;
    }

    public class WxPayApi
    {
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private readonly ILogger _logger;

        private readonly WxConfig _config;
        private readonly HttpClient _client;

        private readonly char[] chars;

        public WxPayApi(IHttpClientFactory httpClientFactory, ILogger<WxPayApi> logger, WxConfig config)
        {
            httpClientFactory.Should().NotBeNull();
            logger.Should().NotBeNull();
            config.Should().NotBeNull();

            this._client = httpClientFactory.CreateClient("wx_pay_api");
            this._logger = logger;
            this._config = config;

            this.chars = this.__chars__();
        }

        public WxConfig Config => this._config;

        public string ToUrl(IDictionary<string, string> dict)
        {
            var res = string.Join("&", dict.Select(x => $"{x.Key}={x.Value}"));
            return res;
        }

        public string ToXml(IDictionary<string, object> dict)
        {
            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in dict)
            {
                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else
                {
                    throw new WxPayException("除了string和int类型不能含有其他数据类型");
                }
            }
            xml += "</xml>";
            return xml;
        }

        string __md5_sign__(IDictionary<string, string> param, string key, out string sign_str_out)
        {
            var sign_params = param
                .Where(x => x.Key != "sign" && x.Value?.Length > 0)
                .ToDictionary(x => x.Key, x => x.Value);

            var p = new SortedDictionary<string, string>(sign_params);

            var sign_str = this.ToUrl(p);

            sign_str += $"&key={key}";

            sign_str_out = sign_str;

            var sign = sign_str.ToMD5();

            sign = sign.Replace("-", string.Empty);

            sign = sign.ToUpper();

            return sign;
        }

        public string MD5Sign(IDictionary<string, string> param, out string sign_str)
        {
            var res = this.__md5_sign__(param, this._config.Key, out sign_str);
            return res;
        }

        char[] __chars__()
        {
            var list = new List<char>();

            list.AddList_(Com.Range('a', 'z' + 1).Select(x => (char)x));
            list.AddList_(Com.Range('A', 'Z' + 1).Select(x => (char)x));
            list.AddList_(Com.Range('0', '9' + 1).Select(x => (char)x));

            return list.ToArray();
        }

        public string GetRandomStr()
        {
            var res = Com.Range(20).Select(x => this.random.Choice(this.chars));
            return string.Join(string.Empty, res);
        }

        IDictionary<string, object> __build_unified_order_parameters__(IDictionary<string, object> dict, ref Dictionary<string, object> log)
        {
            var param = new Dictionary<string, object>(dict);

            if (!param.ContainsKey("out_trade_no"))
                throw new WxPayException("缺少订单号");

            if (!param.ContainsKey("body"))
                throw new WxPayException("缺少body参数");

            if (!param.ContainsKey("total_fee"))
                throw new WxPayException("缺少金额");

            if (!param.ContainsKey("trade_type"))
                throw new WxPayException("缺少交易类型参数");

            if (param["trade_type"]?.ToString() == "JSAPI" && !param.ContainsKey("openid"))
            {
                throw new WxPayException("交易类型为jsapi时必须设置openid");
            }
            if (param["trade_type"]?.ToString() == "NATIVE" && !param.ContainsKey("product_id"))
            {
                throw new WxPayException("交易类型为native时必须设置product_id");
            }

            var utc_now = DateTime.UtcNow;
            var beijing_now = utc_now.AddHours(8);

            param["time_start"] = beijing_now.ToString("yyyyMMddHHmmss");
            param["time_expire"] = beijing_now.AddMinutes(5).ToString("yyyyMMddHHmmss");

            param["notify_url"] = this._config.NotifyUrl;
            param["appid"] = this._config.AppID;
            param["mch_id"] = this._config.MchID;
            param["spbill_create_ip"] = this._config.ServerIp;
            param["sign_type"] = this._config.SignType;
            param["nonce_str"] = this.GetRandomStr();

            var sign = this.__md5_sign__(param.ToDictionary(x => x.Key, x => x.Value.ToString()), this._config.Key, out var sign_str);
            param["sign"] = sign;

            log["build_unified_order_param_sign_str"] = sign_str;

            return param;
        }

        public async Task<WxPayUnifiedOrderResponse> UnifiedOrder(IDictionary<string, object> dict)
        {
            dict.Should().NotBeNull().And.NotBeEmpty();
            dict.Any(x => x.Value == null).Should().BeFalse("WxPayData内部含有值为null的字段!");

            var log = new Dictionary<string, object>
            {
                ["input"] = dict
            };

            try
            {
                var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

                log["wx_url"] = url;

                var param = this.__build_unified_order_parameters__(dict, ref log);

                var post_data = this.ToXml(param);

                log["post_data"] = post_data;

                using var data = new StringContent(post_data, Encoding.UTF8, "application/xml");

                using var res = await this._client.PostAsync(url, data);

                log["http_code"] = res.StatusCode.ToString();

                res.EnsureSuccessStatusCode();

                var xml_data = await res.Content.ReadAsStringAsync();

                log["wx_response"] = xml_data;

                var response = this.__parse_response__(xml_data);

                log["wx_response_model"] = response;

                if (!response.IsReturnCodeSuccess())
                    throw new WxPayException("统一下单接口返回code为失败");

                var my_sign = this.__md5_sign__(response.Parameters, this._config.Key, out var sign_str);

                log["unified_response_sign_str"] = sign_str;

                log["my_response_sign"] = my_sign;

                if (response.sign != my_sign)
                {
                    throw new WxPayException("微信下单返回签名值校验失败");
                }

                return response;
            }
            catch (Exception e)
            {
                log["exception"] = e.Message;
                throw new WxPayException("微信支付统一下单失败", e) { Info = log };
            }
        }

        WxPayUnifiedOrderResponse __parse_response__(string xml_data)
        {
            var dict = this.__xml_to_dict__(xml_data);

            var res = new WxPayUnifiedOrderResponse(dict);

            return res;

            //var generator = new Castle.DynamicProxy.ProxyGenerator();

            //var proxied_res = (WxPayUnifiedOrderResponse)generator.CreateClassProxyWithTarget(
            //    classToProxy: typeof(WxPayUnifiedOrderResponse),
            //    target: res,
            //    interceptors: new ResProxy());

            //return proxied_res;
        }

        public WxPayNotifyData ParseNotifyData(string xml_data)
        {
            var dict = this.__xml_to_dict__(xml_data);

            var res = new WxPayNotifyData(dict);

            return res;
        }

        public IDictionary<string, string> __xml_to_dict__(string xml_data)
        {
            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(xml_data);

                var dict = new Dictionary<string, string>();
                foreach (XmlNode node in xml.FirstChild.ChildNodes)
                {
                    if (node is XmlElement el)
                    {
                        dict[el.Name] = el.InnerText;
                    }
                }

                return dict;
            }
            catch (Exception e)
            {
                throw new WxPayException("解析xml失败", e);
            }
        }

        public string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
