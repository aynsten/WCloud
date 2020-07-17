using System;

namespace WCloud.Core.Authentication.Model
{
    [Serializable]
    public class TokenModel
    {
        public virtual string UserUID { get; set; }

        public virtual string ExtData { get; set; }

        public virtual string AccessToken { get; set; }

        public virtual string RefreshToken { get; set; }


        public virtual DateTime ExpireUtc { get; set; }
    }
}
