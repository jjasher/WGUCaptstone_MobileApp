namespace C971
{
    internal static class ValidationHelper
    {
        public static bool IsValidName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2)
                return false;

            foreach (char c in name.Trim())
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-' && c != '\'' && c != '.')
                    return false;
            }
            return true;
        }

        // strips out dashes, spaces, etc. then checks the digit count
        public static bool IsValidPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string digits = "";
            foreach (char c in phone)
            {
                if (char.IsDigit(c))
                    digits += c;
            }

            // 10 digits or 11 with country code (1)
            return digits.Length == 10 || (digits.Length == 11 && digits[0] == '1');
        }

        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var trimmed = email.Trim();
                var addr = new System.Net.Mail.MailAddress(trimmed);
                if (addr.Address != trimmed)
                    return false;

                // Confirms there's a domain with a dot (e.g., example.com)
                var domain = trimmed.Substring(trimmed.LastIndexOf('@') + 1);
                return domain.Contains('.');
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidTitle(string? text, int minLength = 2)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= minLength;
        }
    }
}
