using System.Collections.Generic;

namespace WCloud.Framework.Email
{
    public class EmailModel
    {
        public EmailModel()
        {
            this.EnableSSL = false;
            this.SendPort = 25;
            this.TimeOut = 1000 * 10;
        }

        //发送设置
        public string SmtpServer { get; set; }

        public string PopServer { get; set; }

        public string UserName { get; set; }

        public string SenderName { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public bool EnableSSL { get; set; }

        public int SendPort { get; set; }

        public int TimeOut { get; set; }

        public List<string> ToList { get; set; }

        public List<string> CcList { get; set; }

        public string Subject { get; set; }

        public string MailBody { get; set; }

        public string[] File_attachments { get; set; }
    }
}
