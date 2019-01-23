using System;
using System.IO;
using System.Text;

namespace RemoveCSharpCommentsTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter full name of source file to remove comments:");
            var filePath = Console.ReadLine();

            try
            {
                var inputString = File.ReadAllText(filePath);
                if (inputString.Length > 0)
                {
                    var sb = new StringBuilder(inputString);

                    while (RemoveComments(sb)) { }

                    if (!inputString.Equals(sb.ToString()))
                    {
                        File.WriteAllText(filePath, sb.ToString());
                        Console.WriteLine("Comments removed");
                        return;
                    }
                }
                Console.WriteLine("No comments found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Program failed. Error: {ex.Message}");
            }
        }

        public static bool RemoveComments(StringBuilder sb)
        {
            var str = sb.ToString();
            int startPos1 = str.IndexOf("//");
            int startPos2 = str.IndexOf("/*");
            //  normalize
            startPos1 = startPos1 == -1 ? int.MaxValue : startPos1;
            startPos2 = startPos2 == -1 ? int.MaxValue : startPos2;

            if (startPos1 < startPos2)
            {
                var endPos1 = str.IndexOf("\r\n", startPos1 + 2);
                endPos1 = endPos1 == -1 ? str.Length : endPos1; // normalize for end of file
                endPos1 += RemoveObsoleteNewLineInlineComment(str, startPos1, endPos1) ? 2 : 0;
                sb.Remove(startPos1, endPos1 - startPos1);
                return true;
            }
            else if (startPos2 < startPos1)
            {
                var endPos2 = str.IndexOf("*/", startPos2 + 2);
                endPos2 += RemoveObsoleteNewLineMultilineComment(str, startPos2, endPos2) ? 2 : 0;
                sb.Remove(startPos2, endPos2 - startPos2 + 2);
                return true;
            }

            return false;
        }

        public static bool RemoveObsoleteNewLineInlineComment(string str, int startPos, int endPos)
        {
            // Determinate if need to remove new line for whole line comments
            return
                (startPos == 0 || //start of file
                 (startPos >= 2 && str.Substring(startPos - 2, 2) == "\r\n")) && // before the comment
                endPos != str.Length;
        }

        public static bool RemoveObsoleteNewLineMultilineComment(string str, int startPos, int endPos)
        {
            // Determinate if need to remove new line for whole line comments
            var startNewLine =
                startPos == 0 || //start of file
                (startPos >= 2 && str.Substring(startPos - 2, 2) == "\r\n"); // before the comment
            var endNewLine =
                endPos + 2 < str.Length - 2 && str.Substring(endPos + 2, 2) == "\r\n"; // after comment

            return startNewLine && endNewLine;
        }
    }
}
