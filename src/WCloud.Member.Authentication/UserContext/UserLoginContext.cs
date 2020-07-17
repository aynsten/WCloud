using Lib.helper;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;

namespace WCloud.Member.Authentication.UserContext
{
    [System.Obsolete]
    public class UserLoginContext<T> : ILoginContext<T> where T : class, ILoginModel
    {
        private readonly T _context;
        public UserLoginContext(T context)
        {
            if (ValidateHelper.IsNotEmpty(context.UserID))
                this._context = context;
        }

        public Task<T> GetLoginContextAsync()
        {
            return Task.FromResult(this._context);
        }
    }
}
