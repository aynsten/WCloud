using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace WCloud.Core.Helper
{
    public interface IStringArraySerializer : IAutoRegistered
    {
        string Serialize(string[] permissions);

        string[] Deserialize(string data);
    }

    public class DefaultStringArraySerializer : IStringArraySerializer
    {
        private readonly ILogger _logger;

        public DefaultStringArraySerializer(ILogger<DefaultStringArraySerializer> logger)
        {
            this._logger = logger;
        }


        public string[] Deserialize(string data)
        {
            try
            {
                data ??= "[]";
                var res = data.JsonToEntity<string[]>();
                return res;
            }
            catch (Exception e)
            {
                this._logger.AddWarningLog(msg: $"反序列化失败,{data}=>{typeof(string[]).FullName}", e: e);

                return new string[] { };
            }
        }

        public string Serialize(string[] permissions)
        {
            permissions.Should().NotBeNull();

            var res = permissions.ToJson();

            return res;
        }
    }
}
