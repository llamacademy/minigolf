using UnityEngine;

namespace LlamAcademy.Minigolf.Bus.Events
{
    public struct BallInHoleEvent : IEvent
    {
        public Vector3 Position;
        public int Strokes;

        public BallInHoleEvent(Vector3 position, int strokes)
        {
            Position = position;
            Strokes = strokes;
        }
    }
}
