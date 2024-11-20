namespace PSInzinerija1.Games.VisualMemory.Models
{
    public class PatternCell(PatternValue value, int index, bool pressed = false)
    {
        public PatternValue Value { get; } = value;
        public int Index { get; } = index;
        public bool Pressed { get; set; } = pressed;
    }

}