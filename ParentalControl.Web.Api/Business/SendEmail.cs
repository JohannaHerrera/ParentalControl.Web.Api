using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace ParentalControl.Web.Api.Business
{
    public class SendEmail
    {
        public bool SendEmailRequest(string parentEmail, string body)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("prueba.controlparental.jkn@gmail.com", "JKN", System.Text.Encoding.UTF8);//Correo de salida
                correo.To.Add(parentEmail); //Correo destino
                correo.Subject = "Notificación Control Parental"; //Asunto
                correo.Body = body; //Mensaje del correo
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.High;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com"; //Host del servidor de correo
                smtp.Port = 587; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential("prueba.controlparental.jkn@gmail.com", "JKN123456a");//Cuenta de correo
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true; //True si el servidor de correo permite ssl
                smtp.Send(correo);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}