
namespace WCloud.Member.Shared.Helper
{
    /// <summary>
    /// 把不同格式的电话号码变成统一格式
    /// 方便数据库比对查找
    /// </summary>
    public interface IMobilePhoneFormatter
    {
        string Format(string phone);
    }

    public class DefaultMobilePhoneFormatter : IMobilePhoneFormatter
    {
        public string Format(string phone)
        {
            return phone;
        }
    }
}
