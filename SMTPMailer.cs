using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class SMTPMailer
{
    readonly string smtpHost;
    readonly int    smtpPort;
    readonly string smtpUsername;
    readonly string smtpPassword;

    public string fromAddr = null;
    public string fromName = null;

    public SmtpClient smtpClient { get; private set; } = null;

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

    public MailMessage CreateMailMessage(string toAddr, string subject, string body, bool isHTML=true)
    {
        MailMessage mailMessage = CreateMailMessage(subject, body, isHTML);
        mailMessage.To.Add(toAddr);
        return mailMessage;
    }

    public bool SendMail(MailMessage mailMessage)
    {
        using(smtpClient = new SmtpClient(smtpHost, smtpPort))
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
    
    public async Task SendMailAsync(MailMessage mailMessage, Action<Exception> callback=null)
    {
        using(smtpClient = new SmtpClient(smtpHost, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl   = true;

            try
            {
                smtpClient.SendCompleted += (s, e) => callback(e.Error);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception message: " + ex.Message);
            }
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

    public async Task SendAsync(string toAddr, string subject, string body, bool isHTML=true)
    {
        PreCheck();

        MailMessage mail = CreateMailMessage(subject, body, isHTML);
        mail.To.Add(new MailAddress(toAddr));

        await SendMailAsync(mail);
    }

    public async Task SendAsync(string[] toAddrList, string subject, string body, bool isHTML=true)
    {
        PreCheck();

        MailMessage mail = CreateMailMessage(subject, body, isHTML);
        mail.To.Add(string.Join(",", toAddrList));

        await SendMailAsync(mail);
    }
}
