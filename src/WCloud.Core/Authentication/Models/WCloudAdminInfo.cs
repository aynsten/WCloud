using System.Collections.Generic;
using Lib.helper;

namespace WCloud.Core.Authentication.Model
{
    public class WCloudAdminInfo 
    {
        public string UserID { get; set; }

        /// <summary>
        /// 用户名，应该是手机号
        /// </summary>
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string UserImg { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }

        public bool IsAuthed()
        {
            var res = ValidateHelper.IsNotEmpty(this.UserID);
            return res;
        }
    }
}
