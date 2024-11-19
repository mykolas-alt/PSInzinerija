using System.Text.Json;

using PSInzinerija1.Exceptions;
namespace Frontend.Services
{
    public class WordListAPIService
    {
        private readonly HttpClient _httpClient;

        public WordListAPIService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("BackendApi");
        }

        public async Task<List<string>?> GetWordsFromApiAsync(string fileName)
        {

            HttpResponseMessage? response;
            try
            {
                response = await _httpClient.GetAsync($"api/wordlist/words?fileName={fileName}");
            }
            catch (Exception)
            {
                throw new WordListLoadException("Failed to fetch words from the file.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new WordListLoadException("Failed to fetch words from the file.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var wordList = JsonSerializer.Deserialize<List<string>>(content);

            if (wordList == null)
            {
                throw new WordListLoadException("Failed to deserialize the word list from the file content.");
            }

            return wordList;
        }
    }
}