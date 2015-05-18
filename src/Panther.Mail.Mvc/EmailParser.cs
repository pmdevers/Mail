using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Panther.Mail.Mvc
{
    public class EmailParser : IEmailParser
    {
        public MailMessage Parse(string emailViewOutput, Email email)
        {
            var message = new MailMessage();
            InitializeMailMessage(message, emailViewOutput, email);
            return message;
        }

        private void InitializeMailMessage(MailMessage message, string emailViewOutput, Email email)
        {
            using (var reader = new StringReader(emailViewOutput))
            {
                ParseHeaders(reader, (key, value) => AssignEmailHeaderToMailMessage(key, value, message));

                AssignCommonHeaders(message, email);

                var messageBody = reader.ReadToEnd().Trim();
                
                message.Body = messageBody;

                //if (message.AlternateViews.Count == 0)
                //{
                //    string messageBody = reader.ReadToEnd().Trim();
                //    if (email.ImageEmbedder.HasImages)
                //    {
                //        AlternateView view = AlternateView.CreateAlternateViewFromString(messageBody, new ContentType("text/html"));
                //        email.ImageEmbedder.AddImagesToView(view);
                //        message.AlternateViews.Add(view);
                //        message.Body = "Plain text not available.";
                //        message.IsBodyHtml = false;
                //    }
                //    else
                //    {
                //        message.Body = messageBody;
                //        if (message.Body.StartsWith("<"))
                //        {
                //            message.IsBodyHtml = true;
                //        }
                //    }
                //}
                //AddAttachments(message, email);
            }
        }

        //private void AddAttachments(MailMessage message, Email email)
        //{
        //    foreach (Attachment attachment in email.Attachments)
        //    {
        //        message.Attachments.Add(attachment);
        //    }
        //}

        private string ParseHeadersForContentType(StringReader reader)
        {
            string contentType = null;
            ParseHeaders(reader, (key, value) =>
            {
                if (key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = value;
                }
            });
            return contentType;
        }

        private MemoryStream CreateStreamOfBody(string body)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(body);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void ParseHeaders(StringReader reader, Action<string, string> useKeyAndValue)
        {
            string line;
            while (string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                // Skip over any empty lines before the headers.
            }

            var headerStart = new Regex(@"^\s*([A-Za-z\-]+)\s*:\s*(.*)");
            do
            {
                var match = headerStart.Match(line);
                if (!match.Success)
                {
                    break;
                }

                var key = match.Groups[1].Value.ToLowerInvariant();
                var value = match.Groups[2].Value.TrimEnd();
                useKeyAndValue(key, value);
            }
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()));
        }

        private string GetAlternativeViewName(Email email, string alternativeViewName)
        {
            if (email.ViewName.StartsWith("~"))
            {
                var index = email.ViewName.LastIndexOf('.');
                return email.ViewName.Insert(index + 1, alternativeViewName + ".");
            }
            return email.ViewName + "." + alternativeViewName;
        }

        private bool IsAlternativeViewHeader(string headerName)
        {
            return headerName.Equals("views", StringComparison.OrdinalIgnoreCase);
        }

        private void AssignEmailHeaderToMailMessage(string key, string value, MailMessage message)
        {
            switch (key)
            {
                case "to":
                    message.To.Add(new MailAddress(value));
                    break;
                case "from":
                    message.From = new MailAddress(value);
                    break;
                case "subject":
                    message.Subject = value;
                    break;
                case "cc":
                    message.Cc.Add(new MailAddress(value));
                    break;
                case "bcc":
                    message.Bcc.Add(new MailAddress(value));
                    break;
                case "reply-to":
                    message.ReplyToList.Add(new MailAddress(value));
                    break;
                case "sender":
                    message.Sender = new MailAddress(value);
                    break;
                case "priority":
                    MailPriority priority;
                    if (Enum.TryParse(value, true, out priority))
                    {
                        message.Priority = priority;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid email priority: {value}. It must be High, Medium or Low.");
                    }
                    break;
                case "content-type":
                    var charsetMatch = Regex.Match(value, @"\bcharset\s*=\s*(.*)$");
                    if (charsetMatch.Success)
                    {
                        message.BodyEncoding = Encoding.GetEncoding(charsetMatch.Groups[1].Value);
                    }
                    break;
                default:
                    message.Headers[key] = value;
                    break;
            }
        }

        private void AssignCommonHeaders(MailMessage message, Email email)
        {
            if (message.To.Count == 0)
            {
                AssignCommonHeader<string>(email, "to", to => message.To.Add(new MailAddress(to)));
                AssignCommonHeader<MailAddress>(email, "to", to => message.To.Add(to));
            }
            if (message.From == null)
            {
                AssignCommonHeader<string>(email, "from", from => message.From = new MailAddress(from));
                AssignCommonHeader<MailAddress>(email, "from", from => message.From = from);
            }
            if (message.Cc.Count == 0)
            {
                AssignCommonHeader<string>(email, "cc", cc => message.Cc.Add(new MailAddress(cc)));
                AssignCommonHeader<MailAddress>(email, "cc", cc => message.Cc.Add(cc));
            }
            if (message.Bcc.Count == 0)
            {
                AssignCommonHeader<string>(email, "bcc", bcc => message.Bcc.Add(new MailAddress(bcc)));
                AssignCommonHeader<MailAddress>(email, "bcc", bcc => message.Bcc.Add(bcc));
            }
            if (message.ReplyToList.Count == 0)
            {
                AssignCommonHeader<string>(email, "replyto", replyTo => message.ReplyToList.Add(new MailAddress(replyTo)));
                AssignCommonHeader<MailAddress>(email, "replyto", replyTo => message.ReplyToList.Add(replyTo));
            }
            if (message.Sender == null)
            {
                AssignCommonHeader<string>(email, "sender", sender => message.Sender = new MailAddress(sender));
                AssignCommonHeader<MailAddress>(email, "sender", sender => message.Sender = sender);
            }
            if (string.IsNullOrEmpty(message.Subject))
            {
                AssignCommonHeader<string>(email, "subject", subject => message.Subject = subject);
            }
        }

        private void AssignCommonHeader<T>(Email email, string header, Action<T> assign)
           where T : class
        {
            object value;
            if (email.ViewData.TryGetValue(header, out value))
            {
                var typedValue = value as T;
                if (typedValue != null)
                {
                    assign(typedValue);
                }
            }
        }
    }
}