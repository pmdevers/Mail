using System;

namespace Panther.Mail
{
    public class MailAddress
    {
        #region Fields

        string displayName;
        string to_string;
        //Encoding displayNameEncoding;

        #endregion // Fields

        #region Constructors

        public MailAddress(string address) : this(address, null)
        {
        }

        public MailAddress(string address, string displayName) {

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            if (displayName != null)
                this.displayName = displayName.Trim();
            ParseAddress(address);
        }

        void ParseAddress(string address)
        {
            // 1. Quotes for display name
            address = address.Trim();
            var idx = address.IndexOf('"');
            if (idx != -1)
            {
                if (idx != 0 || address.Length == 1)
                    throw CreateFormatException();

                var closing = address.LastIndexOf('"');
                if (closing == idx)
                    throw CreateFormatException();

                if (displayName == null)
                    displayName = address.Substring(idx + 1, closing - idx - 1).Trim();
                address = address.Substring(closing + 1).Trim();
            }

            // 2. <email>
            idx = address.IndexOf('<');
            if (idx >= 0)
            {
                if (displayName == null)
                    displayName = address.Substring(0, idx).Trim();
                if (address.Length - 1 == idx)
                    throw CreateFormatException();

                var end = address.IndexOf('>', idx + 1);
                if (end == -1)
                    throw CreateFormatException();

                address = address.Substring(idx + 1, end - idx - 1).Trim();
            }
            Address = address;
            // 3. email
            idx = address.IndexOf('@');
            if (idx <= 0)
                throw CreateFormatException();
            if (idx != address.LastIndexOf('@'))
                throw CreateFormatException();

            User = address.Substring(0, idx).Trim();
            if (User.Length == 0)
                throw CreateFormatException();
            Host = address.Substring(idx + 1).Trim();
            if (Host.Length == 0)
                throw CreateFormatException();
        }

        #endregion // Constructors

        #region Properties

        public string Address { get; private set; }

        public string DisplayName => displayName ?? string.Empty;

        public string Host { get; private set; }

        public string User { get; private set; }

        #endregion // Properties

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return (0 == string.Compare(ToString(), obj.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            if (to_string != null)
                return to_string;

            if (!string.IsNullOrEmpty(displayName))
                to_string = $"\"{DisplayName}\" <{Address}>";
            else
                to_string = Address;

            return to_string;
        }

        private static FormatException CreateFormatException()
        {
            return new FormatException("The specified string is not in the "
                                       + "form required for an e-mail address.");
        }

        #endregion // Methods
    }
}
