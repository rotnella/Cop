using System;

namespace BnEGames.Cop.Processor
{
    public static class VerifyArgument
    {
        public static void ThrowIfNullOrWhitespace(string paramName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName, $"Argument '{paramName}' cannot be null or whitespace.");
            }
        }
    }
}
