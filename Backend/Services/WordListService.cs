namespace Backend.Services
{
    public class WordListService
    {
        public async Task<List<string>?> GetWordsFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileContent = await File.ReadAllTextAsync(filePath);
            var words = fileContent
                .Split(new[] { ' ', ',', '.', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.All(c => !char.IsPunctuation(c)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return words;
        }
    }
}