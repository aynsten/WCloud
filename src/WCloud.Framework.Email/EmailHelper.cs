using Lib.helper;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WCloud.Framework.Email
{
    /*
    <add key="SmptServer" value="smtp.epcservices.com.cn" />
    <add key="SmptLoginName" value="r*****@epcservices.com.cn" />
    <add key="SmptPassWord" value="**********" />
    <add key="SmptSenderName" value="EPC_WEBSITE" />
    <add key="SmptEmailAddress" value="reception@epcservices.com.cn" />
    <add key="FeedBackEmail" value="reception@epcservices.com.cn" />
    */

    /// <summary>
    ///Send_Emails 的摘要说明
    /// </summary>
    public static class EmailSender
    {
        private static System.Net.Mail.MailMessage BuildMail(EmailModel model)
        {
            var mail = new System.Net.Mail.MailMessage();
            //收件人
            if (ValidateHelper.IsNotEmpty(model.ToList))
                foreach (var to in model.ToList)
                    mail.To.Add(to);
            //抄送人
            if (ValidateHelper.IsNotEmpty(model.CcList))
                foreach (var cc in model.CcList)
                    model.CcList.Add(cc);
            mail.From = new MailAddress(model.Address, model.SenderName, Encoding.UTF8);
            mail.Subject = model.Subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = model.MailBody;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;//发送html
            mail.Priority = System.Net.Mail.MailPriority.Normal;
            return mail;
        }

        private static SmtpClient BuildSmtp(EmailModel model)
        {
            var smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(model.UserName, model.Password);
            smtp.Host = model.SmtpServer;
            smtp.EnableSsl = model.EnableSSL;
            smtp.Port = model.SendPort;
            smtp.Timeout = model.TimeOut;
            return smtp;
        }

        public static void SendMail(EmailModel model)
        {
            using (var mail = BuildMail(model))
            using (var smtp = BuildSmtp(model))
                smtp.Send(mail);
        }

        public static async Task SendMailAsync(EmailModel model)
        {
            using (var mail = BuildMail(model))
            using (var smtp = BuildSmtp(model))
                await smtp.SendMailAsync(mail);
        }
    }
}