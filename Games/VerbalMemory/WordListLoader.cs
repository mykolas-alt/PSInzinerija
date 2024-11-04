using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PSInzinerija1.Games.VerbalMemory
{
    public static class WordListLoader
    {
        public static async Task<List<string>> GetUniqueWordsFromFile(string filePath)
        {
            List<string> wordList = new List<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    wordList = lines
                                .SelectMany(line => line.Split(new[] { ' ', ',', '.', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                                .Where(word => word.All(c => c != '!' && c != '(' && c != ')' && c != '"' && c != '\''
                                                            && c != '-' && c != '?' && c != '[' && c != ']'
                                                            && c != '{' && c != '}' && c != '/'))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();
                }
                else
                {
                    Console.WriteLine("File not found at path: " + filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }

            return wordList;
        }
    }
}
