using System.Collections;

namespace Frontend.Games.VisualMemory.Models
{
    public enum PatternValue
    {
        Valid,
        Invalid
    }

    public class Pattern : IEnumerable<PatternCell>
    {
        public Pattern()
        {
            GenerateNewPattern();
        }

        public int GridSize { get; private set; } = 3;
        public int Length { get => CurrentPattern.Count; }
        public int ValidCellAmount { get; private set; } = 3;
        private List<PatternCell> CurrentPattern { get; set; } = [];

        public void GenerateNewPattern()
        {
            var rand = new Random();
            int gridElements = GridSize * GridSize;
            CurrentPattern = Enumerable.Range(0, gridElements)
                .Select(i => new PatternCell(i >= ValidCellAmount ? PatternValue.Invalid : PatternValue.Valid, i))
                .OrderBy(_ => rand.Next())
                .ToList();
        }

        public IEnumerator<PatternCell> GetEnumerator()
        {
            return CurrentPattern.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void IncreaseDifficulty()
        {
            ValidCellAmount++;
            if (ValidCellAmount > Length / 2)
            {
                GridSize++;
            }
        }
    }

}