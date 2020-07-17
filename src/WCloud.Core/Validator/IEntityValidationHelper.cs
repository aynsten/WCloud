using System.Linq;

namespace WCloud.Core.Validator
{
    public interface IEntityValidationHelper<T> where T : class
    {
        bool __IsValid__(T model, out string[] messages);

        bool IsValid(T model, out string msg);
    }

    internal abstract class ValidationHelperBase<T> : IEntityValidationHelper<T> where T : class
    {
        public abstract bool __IsValid__(T model, out string[] messages);

        public bool IsValid(T model, out string msg)
        {
            var res = this.__IsValid__(model, out string[] messages);

            msg = messages.FirstOrDefault();
            return res;
        }
    }

}
