using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Panther.Mail
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class SmtpClient : IDisposable
    {
        private string server;
        private int port;

        public SmtpClient()
        {
            
        }

        public SmtpClient(string server, int port)
        {
            this.server = server;
            this.port = port;
        }

        public void SendAsync(MailMessage mailMessage)
        {
            try
            {
                using (var client = new TcpClient(server, port))
                {
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream) { AutoFlush = true }) 
                    {
                        ExpectedResponse(reader.ReadLine(), 220);
                        // Welcome Recieved;

                        writer.WriteLine("HELO " + server);
                        ExpectedResponse(reader.ReadLine(), 250);
                        // Greetings recieved

                        writer.WriteLine("MAIL FROM:<{0}>", mailMessage.From);
                        ExpectedResponse(reader.ReadLine(), 250);
                        // From Address Accepted

                        foreach (var address in mailMessage.To)
                        {
                            writer.WriteLine("RCPT TO:<{0}>", address.Address);
                            ExpectedResponse(reader.ReadLine(), 250);
                        }

                        //TODO: Add Cc and Bccw

                        writer.WriteLine("DATA");
                        ExpectedResponse(reader.ReadLine(), 354);

                        var message = GetMessage(mailMessage);
                        writer.Write(message);
                        ExpectedResponse(reader.ReadLine(), 250);

                        writer.WriteLine("QUIT");
                        var response = reader.ReadLine();
                        if (response != null && response.IndexOf("221", StringComparison.Ordinal) == -1)
                        {
                            throw new SmtpException(response);   
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw new SmtpException("Error sending mail, see inner exception for details.", ex);
            }
        }

        private string GetMessage(MailMessage mailMessage)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Subject: " + mailMessage.Subject);
            foreach (var address in mailMessage.To)
            {
                stringBuilder.AppendLine("To: " + address.Address);
            }
            //TODO: Add Cc and Bcc
            stringBuilder.AppendLine("From: " + mailMessage.From);

            if (mailMessage.IsBodyHtml)
            {
                stringBuilder.AppendLine("MIME-Version: 1.0");
                stringBuilder.AppendLine("Content-Type: text/html;");
                stringBuilder.AppendLine(" charset=\"iso-8859-1\"");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(mailMessage.Body);
            }
            else
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(mailMessage.Body);
            }
            stringBuilder.AppendLine(".");

            return stringBuilder.ToString();
        }

        public void ExpectedResponse(string response, int code)
        {
            if (response.Substring(0, 3) != code.ToString())
            {
                throw new SmtpException(response);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
        }

        public void Send(MailMessage message)
        {
            
        }
    }

    
}
