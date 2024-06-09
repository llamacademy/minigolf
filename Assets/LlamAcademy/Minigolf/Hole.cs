using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [RequireComponent(typeof(Collider))]
    public class Hole : MonoBehaviour
    {
        private int StrokeCount;

        private int FailingStrokes;

        private void OnEnable()
        {
            EventBus<PlayerStrokeEvent>.OnEvent += HandleStroke;
        }

        private void HandleStroke(PlayerStrokeEvent evt)
        {
            StrokeCount++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                EventBus<BallInHoleEvent>.Raise(new BallInHoleEvent(transform.position, StrokeCount));
            }
        }
    }
}
