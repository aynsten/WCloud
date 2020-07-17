using Lib.core;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace WCloud.Member.Authentication
{
    public static class ClaimsSerializerHelper
    {
        private static Encoding _encoding(Encoding encoding) => ConfigHelper.Instance.EncodingOrDefault(encoding);

        public static string SerializeToString_(this ClaimsPrincipal principal, Encoding encoding = null)
        {
            using (var ms = new MemoryStream())
            {
                using (var bs = new BinaryWriter(ms))
                {
                    principal.WriteTo(bs);
                    bs.Flush();

                    var data = _encoding(encoding).GetString(ms.ToArray());

                    return data;
                }
            }
        }

        public static ClaimsPrincipal DeseiralizeToClaimsPrincipal_(this string data, Encoding encoding = null)
        {
            using (var ms_in = new MemoryStream())
            {
                var data_in = _encoding(encoding).GetBytes(data);

                ms_in.Write(data_in, 0, data_in.Length);
                ms_in.Seek(0, SeekOrigin.Begin);

                using (var br = new BinaryReader(ms_in))
                {
                    var p = new ClaimsPrincipal(br);
                    return p;
                }
            }
        }
    }
}
