namespace Panther.Mail.Mvc
{
    public interface IEmailParser
    {
        MailMessage Parse(string rawEmailString, Email email);
    }
}