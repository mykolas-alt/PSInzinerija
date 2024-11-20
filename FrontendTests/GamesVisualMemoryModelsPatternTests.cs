using Frontend.Games.VisualMemory.Models;

namespace FrontendTests
{
    public class PatternTests
    {
        [Fact]
        public void Pattern_InitializesWithCorrectGridSizeAndValidCellAmount()
        {
            var pattern = new Pattern();

            int gridSize = pattern.GridSize;
            int validCellAmount = pattern.ValidCellAmount;

            Assert.Equal(3, gridSize);
            Assert.Equal(3, validCellAmount);
        }

        [Fact]
        public void GenerateNewPattern_CreatesCorrectPatternLength()
        {
            var pattern = new Pattern();

            pattern.GenerateNewPattern();
            int patternLength = pattern.Length;

            Assert.Equal(pattern.GridSize * pattern.GridSize, patternLength);
        }

        [Fact]
        public void GenerateNewPattern_AssignsCorrectNumberOfValidCells()
        {
            var pattern = new Pattern();

            pattern.GenerateNewPattern();
            int validCells = pattern.Count(cell => cell.Value == PatternValue.Valid);

            Assert.Equal(pattern.ValidCellAmount, validCells);
        }

        [Fact]
        public void IncreaseDifficulty_IncreasesValidCellAmount()
        {
            var pattern = new Pattern();
            int initialValidCellAmount = pattern.ValidCellAmount;

            pattern.IncreaseDifficulty();

            Assert.Equal(initialValidCellAmount + 1, pattern.ValidCellAmount);
        }

        [Fact]
        public void IncreaseDifficulty_IncreasesGridSizeWhenThresholdExceeded()
        {
            var pattern = new Pattern();
            int initialGridSize = pattern.GridSize;

            // Call IncreaseDifficulty enough times to trigger a grid size increase
            for (int i = 0; i <= pattern.Length / 2; i++)
            {
                pattern.IncreaseDifficulty();
            }

            Assert.True(pattern.GridSize > initialGridSize);
        }

        [Fact]
        public void Pattern_ImplementsIEnumerableCorrectly()
        {
            var pattern = new Pattern();

            var enumerator = pattern.GetEnumerator();

            Assert.NotNull(enumerator);
            Assert.IsAssignableFrom<System.Collections.IEnumerable>(pattern);
            Assert.IsAssignableFrom<System.Collections.Generic.IEnumerator<PatternCell>>(enumerator);
        }
    }
}