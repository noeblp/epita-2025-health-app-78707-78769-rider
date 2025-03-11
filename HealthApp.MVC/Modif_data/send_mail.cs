using System;
using System.Net;
using System.Net.Mail;

public class send_mail
{

    public static void SendConfirmationEmail(string recipient, string subject, string body)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("hospital.dorset@gmail.com");
            mail.To.Add(recipient);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("hospital.dorset@gmail.com", "cnzi jkns cnug dmeh"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            
        }
    }
}