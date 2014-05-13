using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace SplitMe.Common
{
    public class Utility
    {
        public static void SendPassword(string to, string pass)
        {
            string sub = "Password Changed!!";
            StringBuilder body = new StringBuilder("Dear User,");
            body.Append(Environment.NewLine);
            body.Append("Your new password is " + pass);
            body.Append(Environment.NewLine);
            body.Append(Environment.NewLine);
            body.Append("Regards");
            body.Append(Environment.NewLine);
            body.Append("SpliteMe Team");

            try
            {
                SendMail(to, sub, body.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMail(string to, string subject, string body)
        {
            MailAddress fromAddress = new MailAddress("jakir.new@gmail.com", "Split Me!!");
            MailAddress toAddress = new MailAddress(to);
            const string fromPassword = "Zakirh<>052";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            try
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //method to send email to HOTMAIL
        public static void SendEmail(string to, string subject, string body)
        {
            try
            {
                //Mail Message
                MailMessage mM = new MailMessage();
                //Mail Address
                mM.From = new MailAddress("lyrix@outlook.com");
                //receiver email id
                mM.To.Add(to);
                //subject of the email
                mM.Subject = subject;
                //add the body of the email
                mM.Body = body;
                //mM.IsBodyHtml = true;
                //SMTP client
                SmtpClient sC = new SmtpClient("smtp.live.com");
                //port number for Hot mail
                sC.Port = 25;
                //credentials to login in to hotmail account
                sC.Credentials = new NetworkCredential("lyrix@outlook.com", "Nay8unny1");
                //enabled SSL
                sC.EnableSsl = true;
                //Send an email
                sC.Send(mM);
            }//end of try block
            catch (Exception ex)
            {
                throw ex;
            }//end of catch
        }//end of Email Method HotMail
    }
}
