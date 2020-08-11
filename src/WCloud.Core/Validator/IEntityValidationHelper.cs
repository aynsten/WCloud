using System.Linq;

namespace WCloud.Core.Validator
{
    public interface IEntityValidationHelper
    {
        bool __IsValid__<T>(T model, out string[] messages);

        bool IsValid<T>(T model, out string msg);
    }

    public abstract class ValidationHelperBase : IEntityValidationHelper
    {
        public abstract bool __IsValid__<T>(T model, out string[] messages);

        public bool IsValid<T>(T model, out string msg)
        {
            var res = this.__IsValid__(model, out string[] messages);

            msg = messages.FirstOrDefault();
            return res;
        }
    }

}
