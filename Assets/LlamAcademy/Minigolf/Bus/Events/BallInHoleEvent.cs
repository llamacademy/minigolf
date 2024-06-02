using UnityEngine;

namespace LlamAcademy.Minigolf.Bus.Events
{
    public struct BallInHoleEvent
    {
        public Vector3 Position;
        public int Strokes;
        public Rating PlayerScore;

        public BallInHoleEvent(Vector3 position, int strokes, Rating playerScore)
        {
            Position = position;
            Strokes = strokes;
            PlayerScore = playerScore;
        }
    }
}
