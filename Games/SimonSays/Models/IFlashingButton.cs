namespace PSInzinerija1.Games.SimonSays.Models
{
    public interface IFlashingButton
    {
        bool IsLit { get; set; }
        int Index { get; }
        Task FlashButton(Action? colorChanged, int duration, int delayBeforeFlash, bool disableButton);
    }
}
