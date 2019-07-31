using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    class send_mail : ModelBase
    {

        [model("smtp_settings")]
        [info("  ")]
        public static readonly string Smtp_settings = "Smtp_settings";
       
        [model("")]
        [info("")]
        public static readonly string to_address = "to_address";

        [model("")]
        [info("")]
        public static readonly string Subject = "Subject";
        [model("")]
        [info("")]
        public static readonly string text = "text";

        [model("")]
        [info("будь який філлер кортий заповнить масив елементів для формування даних")]
        public static readonly string source = "source";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(spec[Smtp_settings].V("SmtpServer"));

                mail.From = new MailAddress(spec[Smtp_settings].V("username"));
                mail.To.Add(spec.V(to_address));
                mail.Subject = spec.V(Subject);
                mail.Body = spec.V(text);

                SmtpServer.Port = spec[Smtp_settings]["Port"].intVal;
                SmtpServer.Credentials = new System.Net.NetworkCredential(spec[Smtp_settings].V("username"), spec[Smtp_settings].V("password"));
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                message.Vset("mail Send", "yes");
            }
            catch (Exception ex) //https://myaccount.google.com/lesssecureapps
            {
                message.Vset("Send fail", ex.ToString());
            }
        }
    }

    class smtp_settings : ModelBase
    {
        [model("")]
        [info("intVal   587")]
        public static readonly string Port = "Port";

        [model("")]
        [info("smtp.gmail.com")]
        public static readonly string SmtpServer = "SmtpServer";

        [model("")]
        [info(" ")]
        public static readonly string username = "username";

        [model("")]
        [info(" ")]
        public static readonly string password = "password";

    }

}
