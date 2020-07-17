using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Member.Application.InitData
{
    public interface IInitDataHelper
    {
        void CreateAdminRole();
        void CreateAdmin();
        void SetAdminRoleForAdmin();
    }
}
