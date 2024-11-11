using System.IO;
using System.Linq;
using System.Threading.Tasks;

using PSInzinerija1.Exceptions;

namespace PSInzinerija1.Services
{
    public class WordListService
    {
        public async Task<List<string>> GetWordsFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new WordListLoadException($"File not found: {filePath}");
                }

                var fileContent = await File.ReadAllTextAsync(filePath);

                var words = fileContent
                    .Split(new[] { ' ', ',', '.', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(word => word.All(c => !char.IsPunctuation(c)))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return words;
            }
            catch (WordListLoadException ex)
            {
                Console.WriteLine("WordListLoadException: " + ex.Message);
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error occured: " + ex.Message);
                return [];
            }
        }
    }
}
