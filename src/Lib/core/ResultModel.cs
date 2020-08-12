using FluentAssertions;
using Lib.core;
using Newtonsoft.Json;
using System.Net;

namespace System
{
    public interface IDataContainer<T>
    {
        bool Success { get; set; }

        T Data { get; set; }
    }

    /// <summary>
    /// 接口公共返回值的缩写
    /// </summary>
    public class _ : _<object> { }

    /// <summary>
    /// 接口公共返回值的缩写
    /// </summary>
    public class _<T> : IDataContainer<T>
    {
        [JsonIgnore]
        public virtual bool Error
        {
            get => !this.Success;
            set => this.Success = !value;
        }

        public virtual bool Success { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual string ErrorMsg { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual T Data { get; set; }

        [JsonIgnore]
        public virtual HttpStatusCode? HttpStatusCode { get; set; }

        #region methods

        public virtual _<T> ThrowIfNotSuccess<E>(Func<string, E> ex) where E : Exception
        {
            if (!this.Success)
                throw ex.Invoke(this.ErrorMsg ?? $"{typeof(_<T>).FullName}默认错误信息");

            return this;
        }

        public virtual _<T> ThrowIfNotSuccess() => this.ThrowIfNotSuccess(msg => new MsgException(msg));

        public virtual _<T> SetSuccessData(T data)
        {
            this.Data = data;
            this.Success = true;
            return this;
        }

        public virtual _<T> SetErrorMsg(string msg, string code = null)
        {
            msg.Should().NotBeNullOrEmpty("错误信息不能为空");

            this.ErrorMsg = msg;
            this.ErrorCode = code;
            this.Success = false;
            return this;
        }

        public virtual _<T> SetHttpStatusCode(HttpStatusCode? code)
        {
            this.HttpStatusCode = code;
            return this;
        }

        #endregion

        public static implicit operator bool(_<T> data)
        {
            return data.Success;
        }
    }
}
