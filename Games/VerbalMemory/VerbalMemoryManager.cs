using System.Text.Json;
using System.Text.Json.Nodes;
using PSInzinerija1.Games.VerbalMemory;
using PSInzinerija1.Enums;

namespace PSInzinerija1.Games.VerbalMemory
{
    public class VerbalMemoryManager : IGameManager
    {
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
        public async Task StartNewGame()
        {
            WordList = await WordListLoader.GetUniqueWordsFromFile("wwwroot/GameRules/SimonSaysRules.txt");
            MistakeCount = 0;
            Score = 0;
            WordsShown.Clear();
            GameOver = false;
            ShowRandomWord();
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
                    OnStatisticsChanged?.Invoke();
                }
                GameOver = true;
                WordList.Clear();
                await StartNewGame();
                return;
            }

            Score++;
            WordsShown.Add(CurrentWord);
            ShowRandomWord();
        }

        private void ShowRandomWord()
        {
            if (WordList.Count > 0)
            {
                Random random = new Random();
                bool showSeenWord = random.Next(1, 11) < 3; // 20% chance to show a seen word
                string newWord = CurrentWord;

                while (true)
                {
                    try
                    {
                        if (showSeenWord && WordsShown.Count > 2)
                        {
                            newWord = WordsShown[random.Next(WordsShown.Count)];
                        }
                        else
                        {
                            newWord = WordList[random.Next(WordList.Count)];
                        }

                        if (newWord == CurrentWord)
                        {
                            throw new DuplicateWordException("Selected word is the same as the previous word. Retrying...");
                        }
                        CurrentWord = newWord;
                        break;
                    }
                    catch (DuplicateWordException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
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