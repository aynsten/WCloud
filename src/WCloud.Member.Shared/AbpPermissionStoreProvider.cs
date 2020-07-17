using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WCloud.Member.Shared
{
    public class AbpPermissionStoreProvider : Volo.Abp.Authorization.Permissions.IPermissionStore
    {
        public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            throw new NotImplementedException();
        }
    }
}
