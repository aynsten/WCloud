using Lib.extension;
using System;

namespace WCloud.Member.Shared.Helper
{
    public interface IPasswordHelper
    {
        string Encrypt(string raw, string salt = null);
    }

    public class DefaultPasswordHelper : IPasswordHelper
    {
        public string Encrypt(string raw, string salt = null)
        {
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            var data = $"{raw}.{salt ?? string.Empty}".ToMD5();

            data = data.Replace("-", string.Empty).Trim().ToUpper();

            return data;
        }
    }
}
