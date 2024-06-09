using UnityEngine;

namespace LlamAcademy.Minigolf.Bus.Events
{
    public struct PlayerStrokeEvent : IEvent
    {
        public Vector3 StartPosition;
        public int TotalStrokes;

        public PlayerStrokeEvent(Vector3 startPosition, int strokes)
        {
            StartPosition = startPosition;
            TotalStrokes = strokes;
        }
    }
}
