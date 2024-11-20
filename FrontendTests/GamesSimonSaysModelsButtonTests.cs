using Frontend.Games.SimonSays;
using Frontend.Games.SimonSays.Models;

using Moq;

namespace FrontendTests
{
    public class ButtonTests
    {
        private readonly Mock<SimonSaysManager> _mockGameManager;
        private readonly Mock<Action> _mockAction;
        private readonly Button _button;

        public ButtonTests()
        {
            _mockGameManager = new Mock<SimonSaysManager>();
            _mockAction = new Mock<Action>();
            _mockGameManager.SetupAllProperties(); // Allows property setting in mock
            _button = new Button("Test", 1, _mockGameManager.Object);
        }

        [Fact]
        public async Task OnClick_DoesNothing_WhenGameIsShowingSequence()
        {
            _mockGameManager.Object.IsShowingSequence = true;

            await _button.OnClick(_mockAction.Object);

            _mockAction.Verify(a => a.Invoke(), Times.Never);
            Assert.False(_button.IsLit);
        }

        [Fact]
        public async Task FlashButton_SetsIsLitCorrectly()
        {
            await _button.FlashButton(null, duration: 300);

            Assert.False(_button.IsLit); // Button should be off after flashing
        }

        [Fact]
        public async Task FlashButton_InvokesColorChangedAndDisablesButton()
        {
            var colorChangedCalled = false;
            Action colorChanged = () => { colorChangedCalled = true; };

            await _button.FlashButton(colorChanged, disableButton: true);

            Assert.False(_button.IsLit);
            Assert.True(colorChangedCalled);
            Assert.False(_mockGameManager.Object.IsShowingSequence); // Should reset to false
        }

        [Fact]
        public async Task FlashButton_WaitsForDelayBeforeFlashing()
        {
            var delayBeforeFlash = 200;
            var duration = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await _button.FlashButton(null, delayBeforeFlash: delayBeforeFlash, duration: duration);

            stopwatch.Stop();
            Assert.InRange(stopwatch.ElapsedMilliseconds, delayBeforeFlash, delayBeforeFlash + 50); // Allow for timing margin
        }
    }
}