using Lib.helper;
using WCloud.Core.Validator;

namespace WCloud.Framework.Common.Validator.AttrValidatorImpl
{
    /// <summary>
    /// 使用validator attribute验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EntityAttributeValidatorHelper<T> : ValidationHelperBase<T> where T : class
    {
        public override bool __IsValid__(T model, out string[] messages)
        {
            messages = ValidateHelper.CheckEntity_(model).ToArray();
            return messages.Length <= 0;
        }
    }
}
