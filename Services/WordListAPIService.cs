using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using PSInzinerija1.Exceptions;

namespace PSInzinerija1.Services
{
    public class WordListAPIService
    {
        private readonly HttpClient _httpClient;

        public WordListAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetWordsFromFileAsync(string filePath)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/wordlist/words?filePath={filePath}");

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
            catch (WordListLoadException ex)
            {
                Console.WriteLine($"Error fetching words: {ex.Message}");
                return [];
            }
        }
    }
}