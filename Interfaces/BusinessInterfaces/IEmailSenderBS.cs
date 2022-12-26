using Core;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Interfaces.BusinessInterfaces
{
    public interface IEmailSenderBS : IBaseBS
    {
        void SendEmail(MailMessage mailMessage);
    }
}
