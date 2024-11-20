
using Frontend.Games.VisualMemory.Models;

namespace FrontendTests
{
    public class PatternCellTests
    {
        [Fact]
        public void PatternCell_InitializesWithGivenValues()
        {
            var expectedValue = PatternValue.Valid;
            var expectedIndex = 1;
            var expectedPressed = true;

            var cell = new PatternCell(expectedValue, expectedIndex, expectedPressed);

            Assert.Equal(expectedValue, cell.Value);
            Assert.Equal(expectedIndex, cell.Index);
            Assert.Equal(expectedPressed, cell.Pressed);
        }

        [Fact]
        public void PatternCell_InitializesWithDefaultPressedValueOfFalse()
        {
            var expectedValue = PatternValue.Invalid;
            var expectedIndex = 2;

            var cell = new PatternCell(expectedValue, expectedIndex);

            Assert.Equal(expectedValue, cell.Value);
            Assert.Equal(expectedIndex, cell.Index);
            Assert.False(cell.Pressed); // Default should be false
        }

        [Fact]
        public void PatternCell_SetPressedValue_ModifiesPressedProperty()
        {
            var cell = new PatternCell(PatternValue.Valid, 0);

            cell.Pressed = true;

            Assert.True(cell.Pressed);
        }
    }
}