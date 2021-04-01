namespace Insolvency.Common
{
    public static class IntExtension
    {
        // Return the int's ordinal extension.
        public static string ToOrdinal(this int value)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int lastDigits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (lastDigits < 11 || lastDigits > 13)
            {
                // Check the last digit.
                switch (lastDigits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return extension;
        }
    }
}