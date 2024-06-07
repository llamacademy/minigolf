using System.Linq;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [RequireComponent(typeof(Collider))]
    public class Hole : MonoBehaviour
    {
        [SerializeField] private Par[] ScoreTargets; // todo: replace with Level SO
        private int StrokeCount;

        private int FailingStrokes;

        private void OnEnable()
        {
            Par failingPar = ScoreTargets.FirstOrDefault(par => par.Rating == Rating.Fail);
            if (failingPar == null)
            {
                Debug.LogError("Missing failing par for this level!");
            }
            else
            {
                FailingStrokes = failingPar.Strokes;
            }

            EventBus<PlayerStrokeEvent>.OnEvent += HandleStroke;
        }

        private void HandleStroke(PlayerStrokeEvent evt)
        {
            StrokeCount++;

            if (StrokeCount >= FailingStrokes)
            {
                // raise failure event...after ball stops moving?
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                EventBus<BallInHoleEvent>.Raise(new BallInHoleEvent(transform.position, StrokeCount, CalculateScore()));
            }
        }

        private Rating CalculateScore()
        {
            Par targetPar = ScoreTargets.FirstOrDefault(target => target.Strokes <= StrokeCount);

            return targetPar == null ? Rating.Fail : targetPar.Rating;
        }
    }
}
