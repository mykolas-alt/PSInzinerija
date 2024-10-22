
namespace PSInzinerija1.Components.Pages.VerbalMemory
{
    public partial class VerbalMemory
    {
        public int MistakeCount { get; private set; } = 0;
        public List<string> WordList { get; private set; } = new List<string>();
        public List<string> WordsShown { get; private set; } = new List<string>();
        public bool GameOver { get; private set; } = false;
        private string CurrentWord = string.Empty;
        public int Score { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await StartNewGame();
        }

        public async Task StartNewGame()
        {
            WordList = GetUniqueWordsFromFile("wwwroot/GameRules/SimonSaysRules.txt");
            MistakeCount = 0;
            Score = 0;
            WordsShown.Clear();
            GameOver = false;
            ShowRandomWord();
            await Task.CompletedTask;
        }

        public async Task HandleButtonClick(bool isSeen)
        {
            if (GameOver)
            {
                return;
            }

            if (WordsShown.Contains(CurrentWord))
            {
                if (!isSeen)
                {
                    MistakeCount++;
                }
            }
            else
            {
                if (isSeen)
                {
                    MistakeCount++;
                }
            }

            if (MistakeCount >= 3)
            {
                if (Score > HighScore)
                {
                    HighScore = Score;
                }
                GameOver = true;
                await StartNewGame();
                return;
            }

            Score++;
            WordsShown.Add(CurrentWord);
            ShowRandomWord();
            await Task.CompletedTask;
        }

        private void ShowRandomWord()
        {
            if (WordList.Count > 0)
            {
                Random random = new Random();
                string newWord;
                bool showSeenWord = random.Next(1, 11) < 3; // 20% chance to show a seen word, because the list is large

                if (showSeenWord && WordsShown.Count > 0)
                {
                    do
                    {
                        newWord = WordsShown[random.Next(WordsShown.Count)];
                    }
                    while (newWord == CurrentWord && WordsShown.Count > 1); // Avoiding the same word 2 times in a row
                }
                else
                {
                    do
                    {
                        newWord = WordList[random.Next(WordList.Count)];
                    }
                    while (newWord == CurrentWord && WordList.Count > 1); // Same as above
                }

                CurrentWord = newWord;
            }
        }


        public List<string> GetUniqueWordsFromFile(string filePath)
        {
            List<string> wordList = new List<string>();
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
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
