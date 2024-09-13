using System;
using System.Linq;

namespace Utilities
{
    public static class Utils
    {
        #region chat formatters
        /// <summary>
        /// cut message to maximum characters
        /// </summary>
        public static string CheckMessageLength(string _message, uint chatMaximumCharacters)
        {
            int charCount = 0;
            string newMassage = "";
            foreach (char c in _message)
            {
                newMassage += c;
                charCount++;
                if (charCount >= chatMaximumCharacters) return newMassage;
            }
            return _message;
        }
        public static string DeleteLines(string s, int linesToRemove)
        {
            return s.Split(Environment.NewLine.ToCharArray(),
                           linesToRemove + 1
                ).Skip(linesToRemove)
                .FirstOrDefault();
        }
        public static int GetLineCount(string input)
        {
            int lineCount = 0;

            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '\r':
                        {
                            if (i + 1 < input.Length)
                            {
                                i++;
                                if (input[i] == '\r')
                                {
                                    lineCount += 2;
                                }
                                else
                                {
                                    lineCount++;
                                }
                            }
                            else
                            {

                                lineCount++;
                            }
                        }
                        break;
                    case '\n':
                        lineCount++;
                        break;
                    default:
                        break;
                }
            }
            return lineCount;
        }
        #endregion

    }
}