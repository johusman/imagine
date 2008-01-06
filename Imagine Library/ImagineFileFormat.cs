using System;
using System.Collections.Generic;
using System.Text;

namespace Imagine.Library
{
    public class ImagineFileFormat
    {
        public static Dictionary<string, string> ExtractSections(string input)
        {
            Dictionary<string, string> sections = new Dictionary<string, string>();

            int startPos = 0;
            int outerOpeningBracePos = -1;
            while ((outerOpeningBracePos = input.IndexOf('{', startPos)) != -1)
            {
                int outerClosingBracePos = FindClosingBrace(input, outerOpeningBracePos);
                int sectionLength = outerClosingBracePos - outerOpeningBracePos - 1;
                string sectionName = input.Substring(startPos, outerOpeningBracePos - startPos - 1).Trim();
                string section = input.Substring(outerOpeningBracePos + 1, sectionLength).Trim();
                sections.Add(sectionName, section);
                startPos = outerClosingBracePos + 1;
            }
            return sections;
        }

        private static int FindClosingBrace(string input, int startPos)
        {
            int nextOpeningBracePos = input.IndexOf('{', startPos + 1);
            int nextClosingBracePos = input.IndexOf('}', startPos + 1);

            if (nextClosingBracePos == -1)
                throw new UnmatchedCurlyBracesException();

            if (nextOpeningBracePos == -1 || nextOpeningBracePos > nextClosingBracePos)
                return nextClosingBracePos;
            else
            {
                int innerClosingBracePos = FindClosingBrace(input, nextOpeningBracePos);
                return FindClosingBrace(input, innerClosingBracePos);
            }
        }
    }

    public class UnmatchedCurlyBracesException : Exception { }
}
