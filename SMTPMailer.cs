using System;
using System.Net;
using System.Net.Mail;

class SMTPMailer
{
    readonly string smtpHost;
    readonly int    smtpPort;
    readonly string smtpUsername;
    readonly string smtpPassword;

    public string fromAddr = null;
    public string fromName = null;

    public SMTPMailer(string smtpHost, int smtpPort, string smtpUsername, string smtpPassword)
    {
        this.smtpHost     = smtpHost;
        this.smtpPort     = smtpPort;
        this.smtpUsername = smtpUsername;
        this.smtpPassword = smtpPassword;
    }

    private void PreCheck()
    {
        if(string.IsNullOrEmpty(fromAddr)
        || string.IsNullOrEmpty(fromName))
            throw new Exception("fromAddr or fromName cannot be null or empty!");
    }

    private MailMessage CreateMailMessage(string subject, string body, bool isHTML=true)
    {
        MailMessage mailMessage = new MailMessage
        {
            IsBodyHtml = isHTML,
            From       = new MailAddress(fromAddr, fromName),
            Subject    = subject,
            Body       = body
        };

        return mailMessage;
    }

    private bool SendMail(MailMessage mailMessage)
    {
        using(var smtpClient = new SmtpClient(smtpHost, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl   = true;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception message: " + ex.Message);
            }

            return false;
        }
    }

    public bool Send(string toAddr, string subject, string body, bool isHTML=true)
    { 
        PreCheck();

        MailMessage mail = CreateMailMessage(subject, body, isHTML);
        mail.To.Add(new MailAddress(toAddr));

        return SendMail(mail);
    }

    public bool Send(string[] toAddrList, string subject, string body, bool isHTML=true)
    {
        PreCheck();

        MailMessage mail = CreateMailMessage(subject, body, isHTML);
        mail.To.Add(string.Join(",", toAddrList));

        return SendMail(mail);
    }
}
