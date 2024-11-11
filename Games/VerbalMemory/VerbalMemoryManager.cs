using System.Text.Json;
using System.Text.Json.Nodes;
using PSInzinerija1.Games.VerbalMemory;
using PSInzinerija1.Enums;
using PSInzinerija1.Exceptions;
using PSInzinerija1.Services;

namespace PSInzinerija1.Games.VerbalMemory
{
    public class VerbalMemoryManager : IGameManager
    {
        private readonly WordListAPIService _wordListAPIService;
        public int MistakeCount { get; private set; } = 0;
        public List<string> WordList { get; private set; } = new List<string>();
        public List<string> WordsShown { get; private set; } = new List<string>();
        public bool GameOver { get; private set; } = false;
        public string CurrentWord = string.Empty;
        public int Score { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;
        public event Action? OnStatisticsChanged;

        public AvailableGames GameID => AvailableGames.VerbalMemory;

        public string SerializedStatistics
        {
            get
            {
                var obj = new
                {
                    HighScore
                };
                var json = JsonSerializer.Serialize(obj);

                return json.ToString();
            }
        }
        public VerbalMemoryManager(WordListAPIService wordListAPIService)
        {
            _wordListAPIService = wordListAPIService;
        }
        public async Task StartNewGame()
        {
            try
            {
                WordList = await _wordListAPIService.GetWordsFromFileAsync("wwwroot/GameRules/SimonSaysRules.txt");
            }
            catch (WordListLoadException ex)
            {
                Console.WriteLine(ex.Message);
                GameOver = true;
                return;
            }

            MistakeCount = 0;
            Score = 0;
            WordsShown.Clear();
            GameOver = false;
            ShowRandomWord();
        }

        public async Task HandleNewButtonClick()
        {
            if (GameOver)
            {
                return;
            }

            if (WordsShown.Contains(CurrentWord))
            {
                MistakeCount++;
            }

            await CheckGameOver();
            if (!GameOver)
            {
                Score++;
                WordsShown.Add(CurrentWord);
                ShowRandomWord();
            }
        }

        public async Task HandleSeenButtonClick()
        {
            if (GameOver)
            {
                return;
            }

            if (!WordsShown.Contains(CurrentWord))
            {
                MistakeCount++;
            }

            await CheckGameOver();

            if (!GameOver)
            {
                Score++;
                WordsShown.Add(CurrentWord);
                ShowRandomWord();
            }
        }

        private async Task CheckGameOver()
        {
            if (MistakeCount >= 3)
            {
                if (Score > HighScore)
                {
                    HighScore = Score;
                    OnStatisticsChanged?.Invoke();
                }

                GameOver = true;
                WordList.Clear();
                await StartNewGame();
            }
        }


        private void ShowRandomWord()
        {
            if (WordList.Count > 0)
            {
                Random random = new Random();
                string newWord;
                bool showSeenWord = random.Next(1, 11) < 3; // 20% chance to show a seen word, because the list is large

                if (showSeenWord && WordsShown.Count > 2)
                {
                    do
                    {
                        newWord = WordsShown[random.Next(WordsShown.Count)];
                    }
                    while (newWord == CurrentWord); // Avoiding the same word 2 times in a row
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



        public void LoadStatisticsFromJSON(string? json)
        {
            if (json == null)
            {
                return;
            }

            var jsonObject = JsonNode.Parse(json)?.AsObject();

            if (jsonObject != null && jsonObject[nameof(HighScore)] != null)
            {
                HighScore = jsonObject[nameof(HighScore)].Deserialize<int>();
            }

        }

        public bool SetHighScore(int? highScore)
        {
            if (highScore == null || highScore.Value < HighScore)
            {
                return false;
            }

            HighScore = highScore.Value;
            return true;
        }
    }
}