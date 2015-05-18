using System;
using System.Collections.Generic;
using System.Text;

namespace Panther.Mail
{
    public class MailMessage
    {
        private string body;

        #region Constructors

        public MailMessage()
        {
            To = new MailAddressCollection();
            Bcc = new MailAddressCollection();
            Cc = new MailAddressCollection();
            ReplyToList = new MailAddressCollection();
            Headers = new Dictionary<string, string> { { "MIME-Version", "1.0" } };

        }

        // FIXME: should it throw a FormatException if the addresses are wrong? 
        // (How is it possible to instantiate such a malformed MailAddress?)
        public MailMessage(MailAddress from, MailAddress to) : this()
        {
            if (from == null || to == null)
                throw new ArgumentNullException();

            From = from;

            To.Add(to);
        }

        public MailMessage(string from, string to) : this()
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException(nameof(@from));
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException(nameof(to));

            From = new MailAddress(from);
            foreach (var recipient in to.Split(new char[] { ',' }))
                To.Add(new MailAddress(recipient.Trim()));
        }

        public MailMessage(string from, string to, string subject, string body) : this()
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException(nameof(@from));
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException(nameof(to));

            From = new MailAddress(from);
            foreach (string recipient in to.Split(new char[] { ',' }))
                To.Add(new MailAddress(recipient.Trim()));

            this.body = body;
            Subject = subject;
        }

        #endregion // Constructors

        #region Properties

        public MailAddressCollection Bcc { get; private set; }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public Encoding BodyEncoding { get; set; }

        public MailAddressCollection Cc { get; private set; }

        public MailAddress From { get; set; }

        public Dictionary<string, string> Headers { get; private set; }

        public bool IsBodyHtml
        {

            get
            {
                return body.StartsWith("<");
            }
            
        }

        public MailPriority Priority { get; set; }

        public MailAddressCollection ReplyToList { get; private set; }

        public MailAddress Sender { get; set; }

        public string Subject { get; set; }

        public MailAddressCollection To { get; set; }

        #endregion // Properties
    }
}