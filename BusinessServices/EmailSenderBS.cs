using Interfaces.BusinessInterfaces;
using Interfaces.DALINterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BusinessServices
{
    public class EmailSenderBS : IEmailSenderBS
    {
        ISettingsDAL _settingsDAL;
        public EmailSenderBS(ISettingsDAL settingsDAL)
        {
            _settingsDAL = settingsDAL;
        }

        public void SendEmail(MailMessage mailMessage)
        {
            var settings = _settingsDAL.GetAll();
            string host = settings.Where(x => x.SettingKey == "EmailHost").FirstOrDefault().Settingvalue;
            string fromEmail = settings.Where(x => x.SettingKey == "FromEmail").FirstOrDefault().Settingvalue;
            string fromEmailPassword = settings.Where(x => x.SettingKey == "FromEmailPassword").FirstOrDefault().Settingvalue;
            int port = Convert.ToInt32(settings.Where(x => x.SettingKey == "EmailHostPort").FirstOrDefault().Settingvalue);
            List<string> toEmails = settings.Where(x => x.SettingKey == "ToEmail" && x.IsActive == true).Select(x => x.Settingvalue).ToList();

            mailMessage.From = new MailAddress(fromEmail, fromEmail);
            //mailMessage.IsBodyHtml = true;
            foreach (var toMail in toEmails)
            {
                mailMessage.To.Add(new MailAddress(toMail));
            }

            SmtpClient smtp = new SmtpClient(host, port);
            smtp.Credentials = new NetworkCredential(fromEmail, fromEmailPassword);
            smtp.EnableSsl = true;
            smtp.SendAsync(mailMessage, null);
        }
    }
}
