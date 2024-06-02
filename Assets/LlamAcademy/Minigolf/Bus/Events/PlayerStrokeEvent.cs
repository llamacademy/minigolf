namespace LlamAcademy.Minigolf.Bus.Events
{
    public struct PlayerStrokeEvent
    {
        public int TotalStrokes;

        public PlayerStrokeEvent(int strokes)
        {
            TotalStrokes = strokes;
        }
    }
}
